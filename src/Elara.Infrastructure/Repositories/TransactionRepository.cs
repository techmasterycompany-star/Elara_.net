using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _context.LoyaltyTransactions
                .Where(t => t.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            await _context.LoyaltyTransactions.AddAsync(transaction, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}