using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Payment
{
    public class PaymentWebhookDto
    {
        public string EventType { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? OrderId { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
        public DateTime EventDate { get; set; }
    }

    public class StripeWebhookEventDto
    {
        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;
        public StripeWebhookDataDto Data { get; set; } = null!;
    }

    public class StripeWebhookDataDto
    {
        public StripePaymentIntentDto Object { get; set; } = null!;
    }

    public class StripePaymentIntentDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public long Amount { get; set; }
        public string Currency { get; set; } = null!;
        public Dictionary<string, string>? Metadata { get; set; }
        public string? ClientSecret { get; set; }
        public StripeChargesDto? Charges { get; set; }
    }

    public class StripeChargesDto
    {
        public List<StripeChargeDto> Data { get; set; } = [];
    }

    public class StripeChargeDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? PaymentMethodId { get; set; }
        public StripePaymentMethodDetailsDto? PaymentMethodDetails { get; set; }
    }

    public class StripePaymentMethodDetailsDto
    {
        public string Type { get; set; } = null!;
        public StripeCardDto? Card { get; set; }
    }

    public class StripeCardDto
    {
        public string Brand { get; set; } = null!;
        public string Last4 { get; set; } = null!;
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
    }

    public class PayPalWebhookEventDto
    {
        public string Id { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public PayPalResourceDto Resource { get; set; } = null!;
    }

    public class PayPalResourceDto
    {
        public string Id { get; set; } = null!;
        public string Status { get; set; } = null!;
        public PayPalAmountDto Amount { get; set; } = null!;
        public string? CustomId { get; set; }
        public List<PayPalLinkDto> Links { get; set; } = [];
    }

    public class PayPalAmountDto
    {
        public string Value { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
    }

    public class PayPalLinkDto
    {
        public string Href { get; set; } = null!;
        public string Rel { get; set; } = null!;
        public string Method { get; set; } = null!;
    }
}