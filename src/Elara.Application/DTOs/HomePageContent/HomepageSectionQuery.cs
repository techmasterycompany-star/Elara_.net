using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;

namespace Elara.Application.DTOs.HomePageContent
{
    public enum HomepageSectionSortBy
    {
        Title,
        Type,
        DisplayOrder,
        MaxItems,
        UpdatedAt
    }
    public class HomepageSectionQuery : PaginationRequest<HomepageSectionSortBy>
    {
        public string? Search { get; set; }
        public HomepageSectionType? Type { get; set; }
        public bool? IsActive { get; set; }
    }
}
