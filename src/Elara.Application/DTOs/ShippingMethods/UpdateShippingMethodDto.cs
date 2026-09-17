namespace Elara.Application.DTOs.ShippingMethods
{
    public class UpdateShippingMethodDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateShippingMethodStatusDto
    {
        public bool IsActive { get; set; }
    }
}
