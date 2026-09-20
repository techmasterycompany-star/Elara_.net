using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class SellerOrderDetailsDto
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

        public IEnumerable<SellerOrderItemDto> Items { get; set; } = [];
    }
}
