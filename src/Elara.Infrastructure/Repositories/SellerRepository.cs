using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerProfile;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext _context;

        public SellerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SellerProfile?> GetByIdAsync(long sellerId, bool forPublic = false)
        {
            if(forPublic)
                return await _context.SellerProfiles
                .AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s =>
                    s.Id == sellerId &&
                    !s.IsDeleted &&
                    s.IsApproved);

            return await _context.SellerProfiles.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == sellerId);
        }

        public async Task<SellerProfile?> GetByUserIdAsync(long userId)
        {
            return await _context.SellerProfiles.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<PaginationQueryResult<Product>> GetSellerProductsAsync(long sellerId, GetSellerProductsRequest request)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.SellerProfileId == sellerId && !x.IsDeleted && x.IsActive);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search));
            }

            if (request.HasStock.HasValue)
            {
                query = request.HasStock.Value
                    ? query.Where(p => p.StockQuantity > 0)
                    : query.Where(p => p.StockQuantity == 0);
            }

            var totalCount = await query.CountAsync();

            var descending = request.SortOrder == SortOrderEnum.Desc;
            query = request.SortBy switch
            {
                SellerProductsSortBy.Price => descending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),

                SellerProductsSortBy.Name => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),

                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            var data = await query
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit)
                .Include(p => p.Images)
                .ToListAsync();

            return new PaginationQueryResult<Product>
            {
                Items = data,
                TotalCount = totalCount,
            };
        }
        

        public async Task<PaginationQueryResult<SellerProfile>> GetSellersAsync(GetSellersRequest request)
        {
            var query = _context.SellerProfiles
                    .AsNoTracking()
                    .Where(x => x.IsApproved && !x.IsDeleted)
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.User.FullName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) 
                || x.StoreName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<SellerProfile>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task UpdateProfileAsync(SellerProfile sellerProfile)
        {
            _context.SellerProfiles.Update(sellerProfile);
            await _context.SaveChangesAsync();
        }
    }
}
