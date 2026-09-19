using Elara.Application.DTOs.Common;

namespace Elara.Application.DTOs.ShippingMethods
{
    public enum ShippingMethodSortBy
    {
        Name,
        BaseCost,
        EstimatedDays,
        CreatedAt,
        UpdatedAt
    }
    public class ShippingMethodListRequest : PaginationRequest<ShippingMethodSortBy>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
