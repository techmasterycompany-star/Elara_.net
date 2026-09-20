using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Shipment
{
    public class SellerShipmentQuery : PaginationRequest
    {
        public ShipmentStatus? Status { get; set; }
        public long? OrderId { get; set; }
    }
}
