using Elara.Application.DTOs.Payment;
using Elara.Application.Interfaces.Service;
using Elara.API.Helpers;
using Elara.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text.Json;

namespace Elara.API.Controllers.Payment
{
    [Route("api/v1/webhooks")]
    [ApiController]
    [AllowAnonymous]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IStripePaymentService _stripeService;
        private readonly IPayPalPaymentService _payPalService;

        public PaymentWebhookController(
            IPaymentService paymentService,
            IStripePaymentService stripeService,
            IPayPalPaymentService payPalService)
        {
            _paymentService = paymentService;
            _stripeService = stripeService;
            _payPalService = payPalService;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = Stripe.EventUtility.ParseEvent(json);
                
                PaymentWebhookDto webhook = new PaymentWebhookDto
                {
                    EventType = stripeEvent.Type,
                    TransactionId = GetTransactionIdFromStripeEvent(stripeEvent),
                    Provider = "Stripe",
                    Status = MapStripeStatus(stripeEvent.Type),
                    Amount = GetAmountFromStripeEvent(stripeEvent),
                    Currency = GetCurrencyFromStripeEvent(stripeEvent),
                    EventDate = DateTime.UtcNow,
                    Metadata = GetMetadataFromStripeEvent(stripeEvent)
                };

                if (!string.IsNullOrEmpty(webhook.TransactionId))
                {
                    await _paymentService.HandleWebhookAsync(webhook);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe webhook error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }
        }

        [HttpPost("paypal")]
        public async Task<IActionResult> PayPalWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            
            try
            {
                var payPalEvent = JsonSerializer.Deserialize<PayPalWebhookEventDto>(json);
                
                if (payPalEvent == null)
                    return BadRequest("Invalid PayPal webhook payload");

                var resource = payPalEvent.Resource;
                if (resource == null)
                    return Ok();

                var webhook = new PaymentWebhookDto
                {
                    EventType = payPalEvent.EventType,
                    TransactionId = resource.Id,
                    Provider = "PayPal",
                    Status = MapPayPalStatus(resource.Status),
                    Amount = decimal.Parse(resource.Amount.Value),
                    Currency = resource.Amount.CurrencyCode,
                    EventDate = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object> 
                    { 
                        { "paypal_event_type", payPalEvent.EventType },
                        { "custom_id", resource.CustomId ?? "" }
                    }
                };

                await _paymentService.HandleWebhookAsync(webhook);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"PayPal webhook error: {ex.Message}");
            }
        }

        private string GetTransactionIdFromStripeEvent(Stripe.Event stripeEvent)
        {
            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" or "payment_intent.payment_failed" or "payment_intent.canceled" 
                    => stripeEvent.Data.Object is Stripe.PaymentIntent pi ? pi.Id : "",
                "charge.succeeded" or "charge.failed" or "charge.refunded"
                    => stripeEvent.Data.Object is Stripe.Charge c ? c.PaymentIntentId ?? c.Id : "",
                _ => ""
            };
        }

        private PaymentStatus MapStripeStatus(string eventType)
        {
            return eventType switch
            {
                "payment_intent.succeeded" or "charge.succeeded" => PaymentStatus.Completed,
                "payment_intent.payment_failed" or "charge.failed" => PaymentStatus.Failed,
                "payment_intent.canceled" => PaymentStatus.Cancelled,
                "charge.refunded" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }

        private decimal GetAmountFromStripeEvent(Stripe.Event stripeEvent)
        {
            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" or "payment_intent.payment_failed" or "payment_intent.canceled"
                    => stripeEvent.Data.Object is Stripe.PaymentIntent pi ? pi.Amount / 100m : 0,
                "charge.succeeded" or "charge.failed" or "charge.refunded"
                    => stripeEvent.Data.Object is Stripe.Charge c ? c.Amount / 100m : 0,
                _ => 0
            };
        }

        private string GetCurrencyFromStripeEvent(Stripe.Event stripeEvent)
        {
            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" or "payment_intent.payment_failed" or "payment_intent.canceled"
                    => stripeEvent.Data.Object is Stripe.PaymentIntent pi ? pi.Currency : "usd",
                "charge.succeeded" or "charge.failed" or "charge.refunded"
                    => stripeEvent.Data.Object is Stripe.Charge c ? c.Currency : "usd",
                _ => "usd"
            };
        }

        private Dictionary<string, object>? GetMetadataFromStripeEvent(Stripe.Event stripeEvent)
        {
            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" or "payment_intent.payment_failed" or "payment_intent.canceled"
                    => stripeEvent.Data.Object is Stripe.PaymentIntent pi 
                        ? pi.Metadata?.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value) 
                        : null,
                _ => null
            };
        }

        private PaymentStatus MapPayPalStatus(string status)
        {
            return status switch
            {
                "COMPLETED" => PaymentStatus.Completed,
                "PENDING" => PaymentStatus.Pending,
                "FAILED" or "DECLINED" => PaymentStatus.Failed,
                "CANCELLED" => PaymentStatus.Cancelled,
                "REFUNDED" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };
        }
    }
}