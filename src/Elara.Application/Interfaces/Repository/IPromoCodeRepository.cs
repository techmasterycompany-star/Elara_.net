using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface IPromoCodeRepository
    {
        Task<PromoCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task IncrementUsageAsync(int promoCodeId, CancellationToken cancellationToken = default);
    }
}