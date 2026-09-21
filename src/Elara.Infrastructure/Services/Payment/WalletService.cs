using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Services.Payment
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository walletRepository, ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<WalletBalanceDto> GetBalanceAsync(long userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                wallet = await _walletRepository.CreateAsync(new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    Currency = "USD",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return new WalletBalanceDto
            {
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                Currency = wallet.Currency
            };
        }

        public async Task<WalletResult> DeductBalanceAsync(long userId, decimal amount)
        {
            if (amount <= 0)
                return new WalletResult { Success = false, Message = "Invalid amount" };

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount)
            {
                return new WalletResult 
                { 
                    Success = false, 
                    Message = wallet == null ? "Wallet not found" : "Insufficient balance" 
                };
            }

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = Elara.Domain.Enums.WalletTransactionType.Payment,
                Amount = -amount,
                BalanceAfter = wallet.Balance,
                Description = "Payment for order",
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.UpdateAsync(wallet);
            await _walletRepository.AddTransactionAsync(transaction);

            return new WalletResult
            {
                Success = true,
                TransactionId = transaction.Id.ToString()
            };
        }

        public async Task<WalletResult> AddBalanceAsync(long userId, decimal amount, string description)
        {
            if (amount <= 0)
                return new WalletResult { Success = false, Message = "Invalid amount" };

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                wallet = await _walletRepository.CreateAsync(new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    Currency = "USD",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = Elara.Domain.Enums.WalletTransactionType.TopUp,
                Amount = amount,
                BalanceAfter = wallet.Balance,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.UpdateAsync(wallet);
            await _walletRepository.AddTransactionAsync(transaction);

            return new WalletResult
            {
                Success = true,
                TransactionId = transaction.Id.ToString()
            };
        }

        public async Task<WalletResult> RefundBalanceAsync(long userId, decimal amount)
        {
            if (amount <= 0)
                return new WalletResult { Success = false, Message = "Invalid amount" };

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return new WalletResult { Success = false, Message = "Wallet not found" };
            }

            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Type = Elara.Domain.Enums.WalletTransactionType.Refund,
                Amount = amount,
                BalanceAfter = wallet.Balance,
                Description = "Refund for order",
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.UpdateAsync(wallet);
            await _walletRepository.AddTransactionAsync(transaction);

            return new WalletResult
            {
                Success = true,
                TransactionId = transaction.Id.ToString()
            };
        }

        public async Task<IEnumerable<WalletTransactionDto>> GetTransactionsAsync(long userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return [];

            var transactions = await _walletRepository.GetTransactionsAsync(wallet.Id);
            return transactions.Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                UserId = userId,
                Type = (Elara.Application.Interfaces.Service.WalletTransactionType)t.Type,
                Amount = t.Amount,
                Description = t.Description,
                BalanceAfter = t.BalanceAfter,
                CreatedAt = t.CreatedAt
            });
        }
    }
}