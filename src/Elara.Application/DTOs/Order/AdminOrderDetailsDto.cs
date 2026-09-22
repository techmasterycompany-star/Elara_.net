using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class AdminOrderDetailsDto
    {
        public long Id { get; set; }

        public CustomerSummaryDto? Customer { get; set; }

        public ShippingAddressDto ShippingAddress { get; set; } = null!;

        public ShippingMethodSummaryDto? ShippingMethod { get; set; }

        public PromoCodeSummaryDto? PromoCode { get; set; }

        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public List<AdminOrderItemDto> Items { get; set; } = [];
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = [];
        public List<AdminShipmentSummaryDto> Shipments { get; set; } = [];

        public PaymentSummaryDto? Payment { get; set; }
    }
}
