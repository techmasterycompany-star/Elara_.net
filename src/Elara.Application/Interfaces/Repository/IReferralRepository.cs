using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface IReferralRepository
    {
        Task<Referral?> GetByReferredUserIdAsync(long referredUserId, CancellationToken cancellationToken = default);
        Task<List<Referral>> GetByReferrerUserIdAsync(long referrerUserId, CancellationToken cancellationToken = default);
        Task AddAsync(Referral referral, CancellationToken cancellationToken = default);
    }
}