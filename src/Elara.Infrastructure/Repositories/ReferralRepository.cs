using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class ReferralRepository : IReferralRepository
    {
        private readonly AppDbContext _context;

        public ReferralRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Referral?> GetByReferredUserIdAsync(long referredUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Referrals
                .FirstOrDefaultAsync(r => r.ReferredUserId == referredUserId, cancellationToken);
        }

        public async Task<List<Referral>> GetByReferrerUserIdAsync(long referrerUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Referrals
                .Where(r => r.ReferrerUserId == referrerUserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Referral referral, CancellationToken cancellationToken = default)
        {
            await _context.Referrals.AddAsync(referral, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}