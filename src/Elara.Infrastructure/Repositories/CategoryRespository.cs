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

        public async Task<PaginationQueryResult<Category>> GetAllCategoriesAsync(CategoryListRequest categoryRequest)
        {
            var query = _context.Categories
                .Where(c => c.IsDeleted == false)
                .Include(c => c.ParentCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryRequest.Search))
            {
                query = query.Where(c => c.Name.Contains(categoryRequest.Search));
            }

            if (categoryRequest.ParentCategoryId.HasValue)
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

            var totalCount = await query.CountAsync();
            var skip = (categoryRequest.PageNumber - 1) * categoryRequest.Limit;

            var items = await query
                .Skip(skip)
                .Take(categoryRequest.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Category>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Category?> GetCategoryByIdAsync(long id)
        {
            return await _context.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
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
            return _context.Categories.AnyAsync(c => !c.IsDeleted && c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsDescendantAsync(long categoryId, long potentialAncestorId)
        {
            var currentId = potentialAncestorId;

            while (true)
            {
                var parentId = await _context.Categories.Where(c => c.Id == currentId && !c.IsDeleted).Select(c => c.ParentCategoryId).FirstOrDefaultAsync();

                if (!parentId.HasValue)
                    return false;

                if (parentId.Value == categoryId)
                    return true;

                currentId = parentId.Value;
            }
        }

        public async Task<bool> HasActiveChildrenAsync(long categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.ParentCategoryId == categoryId && !c.IsDeleted);
        }

        public async Task<bool> HasActiveProductsAsync(long categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted && p.IsActive);
        }
    }
}
