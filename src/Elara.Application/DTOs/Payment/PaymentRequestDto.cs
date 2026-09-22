using System.ComponentModel.DataAnnotations;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Payment
{
    public class PaymentRequestDto
    {
        [Required]
        public long OrderId { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; } = "USD";

        public string? ReturnUrl { get; set; }

        public string? CancelUrl { get; set; }

        // Card details for Stripe
        public CardDetailsDto? CardDetails { get; set; }

        // PayPal details
        public string? PayPalEmail { get; set; }

        // Wallet payment
        public bool UseWalletBalance { get; set; } = false;
    }

    public class CardDetailsDto
    {
        [Required]
        public string CardNumber { get; set; } = null!;

        [Required]
        public string ExpiryMonth { get; set; } = null!;

        [Required]
        public string ExpiryYear { get; set; } = null!;

        [Required]
        public string Cvc { get; set; } = null!;

        public string? CardholderName { get; set; }
    }
}