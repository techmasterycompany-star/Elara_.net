using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.HomePageContent
{
    public class AdminBannerQuery : PaginationRequest
    {
        public string? Search { get; set; }
        public BannerPosition? Position { get; set; }
        public bool? IsActive { get; set; }
    }
}
