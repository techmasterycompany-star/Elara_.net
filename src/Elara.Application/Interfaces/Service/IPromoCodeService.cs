using Elara.Application.DTOs;
using Elara.Application.DTOs.Common;

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

        Task<PaginatedResponse<PromoCodeDto>> GetAdminPromoCodesAsync(PromoCodeQuery query);
        Task<PromoCodeDto> GetAdminPromoCodeByIdAsync(long promoCodeId);
        Task<PromoCodeDto> CreatePromoCodeAsync(CreatePromoCodeDto dto);
        Task<PromoCodeDto> UpdatePromoCodeAsync(long promoCodeId, UpdatePromoCodeDto dto);
        Task UpdatePromoCodeStatusAsync(long promoCodeId, bool isActive);
        Task DeletePromoCodeAsync(long promoCodeId);
    }
}