using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public long PaymentId { get; set; }
        public long OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public PaymentMethodType PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? TransactionId { get; set; }
        public string? Provider { get; set; }
        
        // For redirect-based payments (Stripe, PayPal)
        public string? RedirectUrl { get; set; }
        public string? ClientSecret { get; set; } // For Stripe PaymentIntent
        
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        
        // Order details after successful payment
        public OrderSummaryDto? Order { get; set; }
    }

    public class OrderSummaryDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}