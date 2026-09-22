using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public class UpdateSellerShipmentDto
    {
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        public ShipmentStatus? Status { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
