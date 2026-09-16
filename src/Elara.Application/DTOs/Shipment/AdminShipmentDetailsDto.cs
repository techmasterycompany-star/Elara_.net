using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public class AdminShipmentDetailsDto
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long SellerProfileId { get; set; }
        public string SellerName { get; set; } = null!;

        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public ShipmentStatus Status { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public List<AdminShipmentItemDto> Items { get; set; } = [];
    }
}
