using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class CustomerOrderDetailsDto
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public string ShippingFullName { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingState { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
        public string ShippingCountry { get; set; } = null!;

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<CustomerOrderItemDto> Items { get; set; } = [];
    }
}
