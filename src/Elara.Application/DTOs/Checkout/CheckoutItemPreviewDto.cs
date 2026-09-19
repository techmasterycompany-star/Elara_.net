namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutItemPreviewDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public string SellerName { get; set; } = null!;
    }
}
