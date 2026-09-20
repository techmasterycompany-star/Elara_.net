using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromoCodesController : ControllerBase
    {
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodesController(IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _promoCodeService.GetByCodeAsync(code);
            return result != null
                ? Ok(result)
                : NotFound(new { errorCode = "PROMO_NOT_FOUND", errorMessage = "This promo code does not exist." });
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidatePromoCodeRequest request)
        {
            var result = await _promoCodeService.ValidateAsync(request.Code, request.OrderSubTotal);
            return result.IsValid ? Ok(result) : BadRequest(result);
        }
    }

    public class ValidatePromoCodeRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderSubTotal { get; set; }
    }
}