using Elara.Application.DTOs;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class PromoCodeRepository : IPromoCodeRepository
    {
        private readonly AppDbContext _context;

        public PromoCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PromoCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
        }

        public async Task IncrementUsageAsync(int promoCodeId, CancellationToken cancellationToken = default)
        {
            var promoCode = await _context.PromoCodes.FindAsync(new object[] { promoCodeId }, cancellationToken);
            if (promoCode != null)
            {
                promoCode.TimesUsed += 1;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<PaginationQueryResult<PromoCode>> GetAdminPromoCodesAsync(PromoCodeQuery query)
        {
            var promoCodes = _context.PromoCodes.AsNoTracking().Where(p => !p.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                promoCodes = promoCodes.Where(p => p.Code.Contains(search));
            }

            if (query.DiscountType.HasValue)
                promoCodes = promoCodes.Where(p => p.DiscountType == query.DiscountType.Value);

            if (query.IsActive.HasValue)
                promoCodes = promoCodes.Where(p => p.IsActive == query.IsActive.Value);

            var desc = query.SortOrder == SortOrderEnum.Desc;

            promoCodes = query.SortBy switch
            {
                PromoCodeOrderBy.Code => desc ? promoCodes.OrderByDescending(p => p.Code) : promoCodes.OrderBy(p => p.Code),
                PromoCodeOrderBy.DiscountValue => desc ? promoCodes.OrderByDescending(p => p.DiscountValue) : promoCodes.OrderBy(p => p.DiscountValue),
                PromoCodeOrderBy.ExpiryDate => desc ? promoCodes.OrderByDescending(p => p.ExpiryDate) : promoCodes.OrderBy(p => p.ExpiryDate),
                PromoCodeOrderBy.TimesUsed => desc ? promoCodes.OrderByDescending(p => p.TimesUsed) : promoCodes.OrderBy(p => p.TimesUsed),
                PromoCodeOrderBy.IsActive => desc ? promoCodes.OrderByDescending(p => p.IsActive) : promoCodes.OrderBy(p => p.IsActive),
                PromoCodeOrderBy.CreatedAt => desc ? promoCodes.OrderByDescending(p => p.CreatedAt) : promoCodes.OrderBy(p => p.CreatedAt),
                _ => desc ? promoCodes.OrderByDescending(p => p.CreatedAt) : promoCodes.OrderBy(p => p.CreatedAt)
            };

            var totalCount = await promoCodes.CountAsync();

            var items = await promoCodes.Skip((query.PageNumber - 1) * query.Limit).Take(query.Limit).ToListAsync();

            return new PaginationQueryResult<PromoCode> { Items = items, TotalCount = totalCount };
        }

        public Task<PromoCode?> GetByIdAsync(long promoCodeId, bool includeDeleted = false)
        {
            var query = _context.PromoCodes.AsQueryable();

            if (!includeDeleted)
                query = query.Where(p => !p.IsDeleted);

            return query.FirstOrDefaultAsync(p => p.Id == promoCodeId);
        }

        public Task<bool> CodeExistsAsync(string code, long? excludeId = null)
        {
            var normalizedCode = code.Trim().ToUpper();

            return _context.PromoCodes.AnyAsync(p => !p.IsDeleted && p.Code == normalizedCode && (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task AddAsync(PromoCode promoCode)
        {
            await _context.PromoCodes.AddAsync(promoCode);
        }

        public Task UpdateAsync(PromoCode promoCode)
        {
            _context.PromoCodes.Update(promoCode);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}