using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Shipment : SoftDelete
    {
        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public long SellerProfileId { get; set; }
        public SellerProfile SellerProfile { get; set; } = null!;

        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public ShipmentStatus Status { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public ICollection<ShipmentItem> Items { get; set; } = [];
    }
}
