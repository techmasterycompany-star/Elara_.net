namespace Elara.Application.DTOs.Checkout
{
    public class ShippingMethodDto
    {
        public long ShippingMethodId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; }
    }
}
