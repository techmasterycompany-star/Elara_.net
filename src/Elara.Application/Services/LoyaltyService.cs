using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ITransactionRepository _transactionRepository;

        public LoyaltyService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<LoyaltyBalanceDto> GetBalanceAsync(long userId, CancellationToken cancellationToken = default)
        {
            var balance = await CalculateBalanceAsync(userId, cancellationToken);
            return new LoyaltyBalanceDto { Balance = balance };
        }

        public async Task<List<LoyaltyTransactionDto>> GetTransactionsAsync(long userId, CancellationToken cancellationToken = default)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId, cancellationToken);

            return transactions
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new LoyaltyTransactionDto
                {
                    Points = t.Points,
                    Type = t.Type.ToString(),
                    OrderId = t.OrderId,
                    CreatedAt = t.CreatedAt
                })
                .ToList();
        }

        public async Task<RedeemPointsResultDto> RedeemAsync(long userId, int points, CancellationToken cancellationToken = default)
        {
            if (points <= 0)
                return Invalid("Points to redeem must be greater than zero.");

            var currentBalance = await CalculateBalanceAsync(userId, cancellationToken);

            if (points > currentBalance)
                return Invalid("You do not have enough points to redeem this amount.");

            var transaction = new Transaction
            {
                UserId = userId,
                Points = points,
                Type = TransactionType.Redeemed,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction, cancellationToken);

            return new RedeemPointsResultDto
            {
                IsSuccess = true,
                RemainingBalance = currentBalance - points
            };
        }

        private async Task<int> CalculateBalanceAsync(long userId, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId, cancellationToken);

            var earned = transactions.Where(t => t.Type == TransactionType.Earned).Sum(t => t.Points);
            var redeemed = transactions.Where(t => t.Type == TransactionType.Redeemed).Sum(t => t.Points);

            return earned - redeemed;
        }

        private static RedeemPointsResultDto Invalid(string message) => new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}