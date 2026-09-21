using Elara.Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Elara.Infrastructure.Services.Payment
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(IConfiguration configuration, ILogger<StripePaymentService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var secretKey = _configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey))
            {
                StripeConfiguration.ApiKey = secretKey;
            }
        }

        public async Task<StripePaymentResponse> CreatePaymentIntentAsync(StripePaymentRequest request)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = request.Currency.ToLower(),
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "order_id", request.OrderId.ToString() }
                    },
                    ReturnUrl = request.ReturnUrl,
                    SetupFutureUsage = "off_session"
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return new StripePaymentResponse
                {
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    RequiresAction = paymentIntent.Status == "requires_action"
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating payment intent for order {OrderId}", request.OrderId);
                throw new InvalidOperationException($"Stripe error: {ex.Message}", ex);
            }
        }

        public async Task<bool> RefundAsync(string paymentIntentId, decimal? amount = null)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId
                };

                if (amount.HasValue)
                {
                    options.Amount = (long)(amount.Value * 100);
                }

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                return refund.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error refunding payment intent {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }

        public async Task<StripePaymentIntentDto?> GetPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                return new StripePaymentIntentDto
                {
                    Id = paymentIntent.Id,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    Metadata = paymentIntent.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    ClientSecret = paymentIntent.ClientSecret
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error getting payment intent {PaymentIntentId}", paymentIntentId);
                return null;
            }
        }
    }
}