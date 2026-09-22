using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class ShippingMethod : SoftDelete
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Order> Orders { get; set; } = [];
    }
}
