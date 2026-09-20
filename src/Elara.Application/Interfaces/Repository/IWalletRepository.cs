using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(long userId);
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetTransactionsAsync(long walletId);
    }
}