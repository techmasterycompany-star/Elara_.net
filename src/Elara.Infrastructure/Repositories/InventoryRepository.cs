using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Inventory;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SellerProfile?> GetSellerProfileByUserIdAsync(long userId)
        {
            return await _context.SellerProfiles
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.UserId == userId && !sp.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetSellerInventoryAsync(long sellerProfileId, GetInventoryRequest request)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.SellerProfileId == sellerProfileId && !p.IsDeleted)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            // Pagination
            query = query
                .OrderByDescending(p => p.UpdatedAt)
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit);

            return await query.ToListAsync();
        }

        public async Task<int> GetSellerInventoryCountAsync(long sellerProfileId, GetInventoryRequest request)
        {
            var query = _context.Products
                .Where(p => p.SellerProfileId == sellerProfileId && !p.IsDeleted)
                .AsQueryable();

            // Apply same filters as GetSellerInventoryAsync
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetLowStockProductsCountAsync(long sellerProfileId, int threshold)
        {
            return await _context.Products
                .Where(p => p.SellerProfileId == sellerProfileId
                    && !p.IsDeleted
                    && p.IsActive
                    && p.StockQuantity <= threshold
                    && p.StockQuantity >= 0)
                .CountAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(long sellerProfileId, int threshold = 10, PaginationRequest? request = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.SellerProfileId == sellerProfileId
                    && !p.IsDeleted
                    && p.IsActive
                    && p.StockQuantity <= threshold
                    && p.StockQuantity >= 0)
                .OrderBy(p => p.StockQuantity)
                .AsQueryable();

            if (request != null)
            {
                query = query
                    .Skip((request.PageNumber - 1) * request.Limit)
                    .Take(request.Limit);
            }

            return await query.ToListAsync();
        }

        public async Task<Product?> GetProductStockAsync(long productId, long sellerProfileId)
        {
            return await _context.Products
                .Where(p => p.Id == productId
                    && p.SellerProfileId == sellerProfileId
                    && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProductStockAsync(long productId, long sellerProfileId, int stockQuantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId
                    && p.SellerProfileId == sellerProfileId
                    && !p.IsDeleted);

            if (product == null)
                return false;

            product.StockQuantity = stockQuantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
