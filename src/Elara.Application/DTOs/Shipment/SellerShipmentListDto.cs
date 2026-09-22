using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public class SellerShipmentListDto
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public ShipmentStatus Status { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
