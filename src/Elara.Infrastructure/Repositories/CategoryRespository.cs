using Azure.Core;
using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CategoryListRequest categoryRequest)
        {
            var query = _context.Categories.Where(c => c.IsDeleted == false).AsQueryable();

            if (!string.IsNullOrEmpty(categoryRequest.Search))
            {
                query = query.Where(c => c.Name.Contains(categoryRequest.Search, StringComparison.OrdinalIgnoreCase));
            }

            if(categoryRequest.ParentCategoryId.HasValue)
            {
                query = query.Where(c => c.ParentCategoryId == categoryRequest.ParentCategoryId.Value);
            }

            var descending = categoryRequest.SortOrder == SortOrderEnum.Desc;
            query = categoryRequest.SortBy switch
            {
                CategorySortBy.Name => descending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                CategorySortBy.CreatedAt => descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                CategorySortBy.UpdatedAt => descending ? query.OrderByDescending(c => c.UpdatedAt) : query.OrderBy(c => c.UpdatedAt),
                _ => query
            };

            var skip = (categoryRequest.PageNumber - 1) * categoryRequest.Limit;

            return await query
                .Skip(skip)
                .Take(categoryRequest.Limit)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(long id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }        

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CategoryNameExistsAsync(string name, long? excludeId = null)
        {
            return _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
        }
    }
}
