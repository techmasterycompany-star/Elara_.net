using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public class AdminShipmentListDto
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
        public long SellerId { get; set; }
        public string SellerName { get; set; } = null!;

        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
