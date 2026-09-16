namespace Elara.Application.DTOs.Cart
{
    public class CartDto
    {
        public long CartId { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = [];
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
