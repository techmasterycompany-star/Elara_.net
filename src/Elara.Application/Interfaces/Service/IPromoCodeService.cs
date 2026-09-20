using Elara.Application.DTOs;

namespace Elara.Application.Interfaces
{
    public interface IPromoCodeService
    {
        Task<PromoCodeValidationResultDto> ValidateAsync(
            string code,
            decimal orderSubTotal,
            CancellationToken cancellationToken = default);

        Task<PromoCodeDetailsDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);
    }
}