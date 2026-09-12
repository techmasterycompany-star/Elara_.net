using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class OrderStatusHistory : BaseEntity
    {
        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string Status { get; set; } = null!;
        public string Notes { get; set; } = null!;
    }
}
