using Elara.Application.DTOs;

namespace Elara.Application.Interfaces
{
    public interface ILoyaltyService
    {
        Task<LoyaltyBalanceDto> GetBalanceAsync(long userId, CancellationToken cancellationToken = default);

        Task<List<LoyaltyTransactionDto>> GetTransactionsAsync(long userId, CancellationToken cancellationToken = default);

        Task<RedeemPointsResultDto> RedeemAsync(long userId, int points, CancellationToken cancellationToken = default);
    }
}