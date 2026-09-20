using Elara.Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Elara.Infrastructure.Services.Payment
{
    public class PayPalPaymentService : IPayPalPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayPalPaymentService> _logger;
        private readonly HttpClient _httpClient;
        private string? _accessToken;
        private DateTime _tokenExpiry;

        public PayPalPaymentService(
            IConfiguration configuration, 
            ILogger<PayPalPaymentService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            
            var baseUrl = _configuration["PayPal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _accessToken;
            }

            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("PayPal credentials not configured");
            }

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            
            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Basic", authToken) },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<PayPalTokenResponse>(content);
            
            _accessToken = tokenResponse?.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds((tokenResponse?.ExpiresIn ?? 3600) - 60);
            
            return _accessToken!;
        }

        public async Task<PayPalPaymentResponse> CreateOrderAsync(PayPalPaymentRequest request)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var orderRequest = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            reference_id = request.OrderId.ToString(),
                            amount = new
                            {
                                currency_code = request.Currency,
                                value = request.Amount.ToString("F2")
                            }
                        }
                    },
                    application_context = new
                    {
                        return_url = request.ReturnUrl,
                        cancel_url = request.CancelUrl,
                        brand_name = "Elara",
                        landing_page = "BILLING",
                        user_action = "PAY_NOW"
                    }
                };

                var json = JsonSerializer.Serialize(orderRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/v2/checkout/orders", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal error creating order: {Response}", responseContent);
                    throw new InvalidOperationException($"PayPal error: {responseContent}");
                }

                var orderResponse = JsonSerializer.Deserialize<PayPalOrderCreateResponse>(responseContent);
                
                var approveUrl = orderResponse?.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href;

                return new PayPalPaymentResponse
                {
                    OrderId = orderResponse?.Id ?? "",
                    ApproveUrl = approveUrl ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for order {OrderId}", request.OrderId);
                throw;
            }
        }

        public async Task<bool> CaptureOrderAsync(string orderId)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync($"/v2/checkout/orders/{orderId}/capture", null);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal error capturing order: {Response}", content);
                    return false;
                }

                var captureResponse = JsonSerializer.Deserialize<PayPalOrderCaptureResponse>(content);
                return captureResponse?.Status == "COMPLETED";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order {OrderId}", orderId);
                return false;
            }
        }

        public async Task<bool> RefundAsync(string captureId, decimal? amount = null)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                object refundRequest;
                
                if (amount.HasValue)
                {
                    refundRequest = new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = amount.Value.ToString("F2")
                        }
                    };
                }
                else
                {
                    refundRequest = new { };
                }

                var json = JsonSerializer.Serialize(refundRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/v2/payments/captures/{captureId}/refund", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding PayPal capture {CaptureId}", captureId);
                return false;
            }
        }

        public async Task<PayPalOrderDto?> GetOrderAsync(string orderId)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"/v2/checkout/orders/{orderId}");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<PayPalOrderDto>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PayPal order {OrderId}", orderId);
                return null;
            }
        }

        private class PayPalTokenResponse
        {
            public string? AccessToken { get; set; }
            public int ExpiresIn { get; set; }
        }

        private class PayPalOrderCreateResponse
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
            public List<PayPalLinkDto>? Links { get; set; }
        }

        private class PayPalLinkDto
        {
            public string? Href { get; set; }
            public string? Rel { get; set; }
            public string? Method { get; set; }
        }

        private class PayPalOrderCaptureResponse
        {
            public string? Id { get; set; }
            public string? Status { get; set; }
        }
    }
}