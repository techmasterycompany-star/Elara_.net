namespace Elara.Application.DTOs.Shipment
{
    public class CreateSellerShipmentDto
    {
        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public List<CreateShipmentItemDto> Items { get; set; } = [];
    }
}
