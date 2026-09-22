namespace Elara.Application.DTOs.Order
{
    public class AdminOrderItemDto
    {
        public long Id { get; set; }

        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImageUrl { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }

        // Useful for admin seller filtering and order management
        public long SellerId { get; set; }
        public string SellerName { get; set; } = null!;
    }

}
