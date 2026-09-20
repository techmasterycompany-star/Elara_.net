using Elara.Application.DTOs;

namespace Elara.Application.Interfaces
{
    public interface IReferralService
    {
        ReferralCodeDto GetMyCode(long userId);

        Task<ApplyReferralResultDto> ApplyAsync(
            long referredUserId,
            string code,
            CancellationToken cancellationToken = default);

        Task<List<ReferralHistoryDto>> GetHistoryAsync(
            long referrerUserId,
            CancellationToken cancellationToken = default);
    }
}