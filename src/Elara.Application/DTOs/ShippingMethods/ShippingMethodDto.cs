namespace Elara.Application.DTOs.ShippingMethods
{
    public class ShippingMethodDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
