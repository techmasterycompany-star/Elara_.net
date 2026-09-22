using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public enum ShipmentSortBy
    {
        Id,
        CreatedAt,
        ShippedDate,
        EstimatedDeliveryDate,
        DeliveredDate,
        Status
    }
    public class AdminShipmentFilterDto : PaginationRequest<ShipmentSortBy>
    {
        public string? Search { get; set; }

        public ShipmentStatus? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public long? SellerId { get; set; }
        public long? CustomerId { get; set; }
        public long? OrderId { get; set; }

    }
}
