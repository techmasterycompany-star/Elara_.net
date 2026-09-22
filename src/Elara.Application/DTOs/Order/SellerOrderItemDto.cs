namespace Elara.Application.DTOs.Order
{
    public class SellerOrderItemDto
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public int QuantityShipped { get; set; }
        public int QuantityRemaining { get; set; }
    }
}
