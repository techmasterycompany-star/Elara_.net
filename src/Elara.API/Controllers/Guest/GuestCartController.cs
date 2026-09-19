using Elara.Application.DTOs.Common;
using Elara.Application.Interfaces.Service;
using Elara.Application.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Guest
{
    [Route("api/v1/guest")]
    [ApiController]
    public class GuestCartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public GuestCartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Create a new guest session for shopping
        [HttpPost("session")]
        public async Task<IActionResult> CreateGuestSession()
        {
            var guestSession = await _cartService.CreateGuestSessionAsync();
            Response.Cookies.Append("Guest-Session-Id", guestSession.GuestSessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            var response = ApiResponse<string>.SuccessResponse("Guest session created successfully");
            return Ok(response);
        }
    }
}
