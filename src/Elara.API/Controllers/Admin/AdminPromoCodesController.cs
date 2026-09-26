using Elara.Application.DTOs;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/promo-codes")]
    [Authorize(Roles = "Admin")]
    public class AdminPromoCodesController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;

        public AdminPromoCodesController(IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPromoCodes([FromQuery] PromoCodeQuery query)
        {
            var result = await _promoCodeService.GetAdminPromoCodesAsync(query);
            return Ok(ApiResponse<PaginatedResponse<PromoCodeDto>>.SuccessResponse(result));
        }

        [HttpGet("{promoCodeId:long}")]
        public async Task<IActionResult> GetPromoCode(long promoCodeId)
        {
            var result = await _promoCodeService.GetAdminPromoCodeByIdAsync(promoCodeId);
            return Ok(ApiResponse<PromoCodeDto>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromoCode(CreatePromoCodeDto dto)
        {
            var result = await _promoCodeService.CreatePromoCodeAsync(dto);
            return Ok(ApiResponse<PromoCodeDto>.SuccessResponse(result));
        }

        [HttpPut("{promoCodeId:long}")]
        public async Task<IActionResult> UpdatePromoCode(long promoCodeId, UpdatePromoCodeDto dto)
        {
            var result = await _promoCodeService.UpdatePromoCodeAsync(promoCodeId, dto);
            return Ok(ApiResponse<PromoCodeDto>.SuccessResponse(result));
        }

        [HttpPatch("{promoCodeId:long}/status")]
        public async Task<IActionResult> UpdatePromoCodeStatus(long promoCodeId, bool isActive)
        {
            await _promoCodeService.UpdatePromoCodeStatusAsync(promoCodeId, isActive);
            return Ok(ApiResponse<string>.SuccessResponse("Promo code status updated successfully."));
        }

        [HttpDelete("{promoCodeId:long}")]
        public async Task<IActionResult> DeletePromoCode(long promoCodeId)
        {
            await _promoCodeService.DeletePromoCodeAsync(promoCodeId);
            return Ok(ApiResponse<string>.SuccessResponse("Promo code deleted successfully."));
        }
    }
}
