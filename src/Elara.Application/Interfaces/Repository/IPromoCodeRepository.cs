using Elara.Application.DTOs;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface IPromoCodeRepository
    {
        Task<PromoCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task IncrementUsageAsync(int promoCodeId, CancellationToken cancellationToken = default);
        Task<PaginationQueryResult<PromoCode>> GetAdminPromoCodesAsync(PromoCodeQuery query);
        Task<PromoCode?> GetByIdAsync(long promoCodeId, bool includeDeleted = false);
        Task<bool> CodeExistsAsync(string code, long? excludeId = null);
        Task AddAsync(PromoCode promoCode);
        Task UpdateAsync(PromoCode promoCode);
        Task SaveChangesAsync();
    }
}