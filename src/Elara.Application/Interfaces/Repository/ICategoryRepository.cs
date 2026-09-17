using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface ICategoryRepository 
    { 
        Task<PaginationQueryResult<Category>> GetAllCategoriesAsync(CategoryListRequest categoryRequest);
        Task<Category?> GetCategoryByIdAsync(long id);
        Task<bool> CategoryNameExistsAsync(string name, long? excludeId = null);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
    }
}
