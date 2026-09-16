namespace Elara.Application.DTOs.Shipment
{
    public class AdminUpdateShipmentRequestDto
    {
        public string? Carrier { get; set; }

        public string? TrackingNumber { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
    }
}
