using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.HomePageContent
{
    public class AdminBannerQuery : PaginationRequest<AdminBannerOrderBy>
    {
        public string? Search { get; set; }
        public BannerPosition? Position { get; set; }
        public bool? IsActive { get; set; }
    }

    public enum AdminBannerOrderBy {
        Title,
        Position,
        DisplayOrder,
        StartDate,
        EndDate
    }
}
