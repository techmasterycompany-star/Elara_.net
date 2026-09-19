using Elara.Application.DTOs.Checkout;
using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Elara.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Checkout
{
    [Route("api/v1")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        // Get list of available shipping methods
        [HttpGet("shipping-methods")]
        public async Task<IActionResult> GetShippingMethods()
        {
            var shippingMethods = await _checkoutService.GetShippingMethodsAsync();
            var response = ApiResponse<IEnumerable<ShippingMethodDto>>.SuccessResponse(shippingMethods);
            return Ok(response);
        }

        // Preview checkout totals before placing order
        [HttpPost("checkout/preview")]
        public async Task<IActionResult> PreviewCheckout([FromBody] CheckoutPreviewRequest request)
        {
            var preview = await _checkoutService.PreviewCheckoutAsync(GetUserId(), GetGuestSessionId(), request);
            var response = ApiResponse<CheckoutPreviewResponse>.SuccessResponse(preview);
            return Ok(response);
        }

        // Process checkout and create order
        [HttpPost("checkout")]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequest request)
        {
            var checkoutResponse = await _checkoutService.ProcessCheckoutAsync(GetUserId(), GetGuestSessionId(), request);
            var response = ApiResponse<CheckoutResponse>.SuccessResponse(checkoutResponse);
            return Ok(response);
        }

        private long? GetUserId() => User.Identity?.IsAuthenticated == true
            ? User.GetAuthenticatedUserId()
            : null;

        private string? GetGuestSessionId() => Request.Cookies["Guest-Session-Id"];
    }
}
