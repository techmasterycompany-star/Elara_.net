using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutResponse
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Message { get; set; } = null!;
    }
}
