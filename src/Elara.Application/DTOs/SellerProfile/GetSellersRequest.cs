using Elara.Application.DTOs.Common;

namespace Elara.Application.DTOs.SellerProfile
{
    public enum SellerSortBy
    {
        StoreName,
        CreatedAt
    }
    public class GetSellersRequest : PaginationRequest<SellerSortBy>
    {
        public string? Search { get; set; }

    }
}
