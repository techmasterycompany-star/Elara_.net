namespace Elara.Application.DTOs.Shipment
{
    public class ShipSellerShipmentDto
    {
        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
