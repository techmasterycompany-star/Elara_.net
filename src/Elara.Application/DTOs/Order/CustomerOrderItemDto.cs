namespace Elara.Application.DTOs.Order
{
    public class CustomerOrderItemDto
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
