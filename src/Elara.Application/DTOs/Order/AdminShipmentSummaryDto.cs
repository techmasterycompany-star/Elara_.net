using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class AdminShipmentSummaryDto
    {
        public long Id { get; set; }

        public long SellerId { get; set; }
        public string SellerName { get; set; } = null!;

        public string Carrier { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;

        public ShipmentStatus Status { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public int ItemsCount { get; set; }
    }
}
