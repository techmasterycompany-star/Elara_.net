using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class SellerOrderQuery : PaginationRequest
    {
        public OrderStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
