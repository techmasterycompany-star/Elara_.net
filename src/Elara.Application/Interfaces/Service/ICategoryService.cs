using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;

namespace Elara.Application.Interfaces.Service
{
    public interface ICategoryService
    {
        Task<PaginatedResponse<CategoryDto>> GetAllCategoriesAsync(CategoryListRequest categoryRequest);
        Task<CategoryDto?> GetCategoryByIdAsync(long id);
        Task CreateCategoryAsync(CreateCategoryDto category);
        Task UpdateCategoryAsync(long categoryId, UpdateCategoryDto category);
        Task DeleteCategoryAsync(long id);
    }
}
