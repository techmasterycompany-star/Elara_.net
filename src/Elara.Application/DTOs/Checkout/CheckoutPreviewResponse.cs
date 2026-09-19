namespace Elara.Application.DTOs.Checkout
{
    public class CheckoutPreviewResponse
    {
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CheckoutItemPreviewDto> Items { get; set; } = [];
        public string? AppliedPromoCode { get; set; }
        public ShippingMethodDto ShippingMethod { get; set; } = null!;
    }
}
