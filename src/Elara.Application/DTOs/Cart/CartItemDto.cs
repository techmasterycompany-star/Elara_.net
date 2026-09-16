namespace Elara.Application.DTOs.Cart
{
    public class CartItemDto
    {
        public long CartItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int AvailableStock { get; set; }
        public bool IsActive { get; set; }
    }
}
