using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class SellerOrderListDto
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingCity { get; set; } = null!;
        public string ShippingCountry { get; set; } = null!;
        public int ItemCount { get; set; }
        public decimal SellerSubtotal { get; set; }
    }
}
