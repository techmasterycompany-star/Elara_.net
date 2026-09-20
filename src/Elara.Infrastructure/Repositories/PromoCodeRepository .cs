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
    }
}