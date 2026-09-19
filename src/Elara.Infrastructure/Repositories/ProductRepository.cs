using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Product;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Elara.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationQueryResult<Product>> GetProductsAsync(ProductQuery query, long? sellerProfileId = null)
        {
            var products = _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.SellerProfile)
            .Include(p => p.Images)
            .AsQueryable();

            if (sellerProfileId.HasValue)
            {
                products = products.Where(
                    p => p.SellerProfileId == sellerProfileId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                products = products.Where(p =>
                    p.Name.Contains(query.Search) ||
                    p.Description.Contains(query.Search));
            }

            if (query.CategoryId.HasValue)
            {
                products = products.Where(
                    p => p.CategoryId == query.CategoryId.Value);
            }

            if (query.SellerProfileId.HasValue && !sellerProfileId.HasValue)
            {
                products = products.Where(
                    p => p.SellerProfileId == query.SellerProfileId.Value);
            }

            if (query.IsActive.HasValue)
            {
                products = products.Where(
                    p => p.IsActive == query.IsActive.Value);
            }

            var desc = query.SortOrder == SortOrderEnum.Desc;

            products = query.SortBy switch
            {
                ProductSortBy.Name => desc ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),

                ProductSortBy.Price => desc ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),

                ProductSortBy.StockQuantity => desc ? products.OrderByDescending(p => p.StockQuantity) : products.OrderBy(p => p.StockQuantity),

                ProductSortBy.UpdatedAt => desc ? products.OrderByDescending(p => p.UpdatedAt) : products.OrderBy(p => p.UpdatedAt),

                _ => desc ? products.OrderByDescending(p => p.CreatedAt) : products.OrderBy(p => p.CreatedAt)
            };

            var totalCount = await products.CountAsync();

            var items = await products
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Product>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Product?> GetByIdAsync(long productId, long? sellerProfileId = null, bool includeDeleted = false)
        {
            var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.SellerProfile)
            .Include(p => p.Images)
            .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(p => !p.IsDeleted);
            }

            if(sellerProfileId.HasValue)
            {
                query = query.Where(p => p.SellerProfileId == sellerProfileId.Value);
            }


            return await query.FirstOrDefaultAsync(p => p.Id == productId);
        }

        public Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            return Task.FromResult(product);
        }

        public Task<bool> ExistsAsync(long productId)
        {
            return _context.Products.AnyAsync(p => p.Id == productId && !p.IsDeleted);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
