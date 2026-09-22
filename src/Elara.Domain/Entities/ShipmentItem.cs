using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class ShipmentItem : BaseEntity
    {
        public long ShipmentId { get; set; }
        public Shipment Shipment { get; set; } = null!;

        public long OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
