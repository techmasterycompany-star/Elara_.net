using Elara.Application.DTOs.Common;

namespace Elara.Application.DTOs.Category
{
    public enum CategorySortBy
    {
        Name,
        CreatedAt,
        UpdatedAt
    }
    public class CategoryListRequest : PaginationRequest<CategorySortBy>
    {
        public string? Search { get; set; }
        public long? ParentCategoryId { get; set; }
    }
}
