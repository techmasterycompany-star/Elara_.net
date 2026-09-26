using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs
{
    public class PromoCodeQuery : PaginationRequest<PromoCodeOrderBy>
    {
        public string? Search { get; set; }
        public DiscountType? DiscountType { get; set; }
        public bool? IsActive { get; set; }
    }
}