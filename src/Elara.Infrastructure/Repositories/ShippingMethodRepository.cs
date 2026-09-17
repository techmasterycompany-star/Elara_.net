using Elara.Application.DTOs.Category;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.ShippingMethods;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class ShippingMethodRepository : IShippingMethodRepository
    {
        private readonly AppDbContext _context;

        public ShippingMethodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShippingMethod>> GetAllShippingMethodsAsync(ShippingMethodListRequest shippingMethodRequest)
        {
            var query = _context.ShippingMethods.Where(s => s.IsDeleted == false).AsQueryable();

            if (!string.IsNullOrEmpty(shippingMethodRequest.Search))
            {
                query = query.Where(s => s.Name.Contains(shippingMethodRequest.Search, StringComparison.OrdinalIgnoreCase));
            }

            if (shippingMethodRequest.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == shippingMethodRequest.IsActive.Value);
            }

            var descending = shippingMethodRequest.SortOrder == SortOrderEnum.Desc;

            query = shippingMethodRequest.SortBy switch
            {
                ShippingMethodSortBy.Name => descending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                ShippingMethodSortBy.BaseCost => descending ? query.OrderByDescending(s => s.BaseCost) : query.OrderBy(s => s.BaseCost),
                ShippingMethodSortBy.EstimatedDays => descending ? query.OrderByDescending(s => s.EstimatedDays) : query.OrderBy(s => s.EstimatedDays),
                ShippingMethodSortBy.CreatedAt => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                ShippingMethodSortBy.UpdatedAt => descending ? query.OrderByDescending(s => s.UpdatedAt) : query.OrderBy(s => s.UpdatedAt),
                _ => query
            };

            var skip = (shippingMethodRequest.PageNumber - 1) * shippingMethodRequest.Limit;

            return await query
                .Skip(skip)
                .Take(shippingMethodRequest.Limit)
                .ToListAsync();

        }

        public async Task<ShippingMethod?> GetShippingMethodByIdAsync(long id)
        {
            return await _context.ShippingMethods.FindAsync(id);
        }

        public async Task CreateShippingMethodAsync(ShippingMethod shippingMethod)
        {
            await _context.ShippingMethods.AddAsync(shippingMethod);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShippingMethodAsync(ShippingMethod shippingMethod)
        {
            _context.ShippingMethods.Update(shippingMethod);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ShippingMethodNameExistsAsync(string name, long? excludeId = null)
        {
            return _context.ShippingMethods
                .AnyAsync(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && (!excludeId.HasValue || s.Id != excludeId.Value));
        }
    }
}
