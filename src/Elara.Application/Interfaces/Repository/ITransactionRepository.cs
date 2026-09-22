using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
        Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    }
}