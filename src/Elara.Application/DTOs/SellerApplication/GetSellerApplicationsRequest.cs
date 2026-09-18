using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.SellerApplication
{
    public class GetSellerApplicationsRequest : PaginationRequest
    {
        public long SellerId { get; set; }
        public SellerApplicationStatus? Status { get; set; }
    }
}
