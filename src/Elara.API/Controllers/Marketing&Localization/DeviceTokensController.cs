using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    [Authorize]
    public class DeviceTokensController : ControllerBase
    {
        private readonly IDeviceTokenService _deviceTokenService;

        public DeviceTokensController(IDeviceTokenService deviceTokenService)
        {
            _deviceTokenService = deviceTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDeviceRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _deviceTokenService.RegisterAsync(userId, request.Token, request.Platform);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("unregister")]
        public async Task<IActionResult> Unregister([FromBody] UnregisterDeviceRequest request)
        {
            var result = await _deviceTokenService.UnregisterAsync(request.Token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.Parse(claim!);
        }
    }

    public class RegisterDeviceRequest
    {
        public string Token { get; set; } = string.Empty;
        public Elara.Domain.Enums.DevicePlatform Platform { get; set; }
    }

    public class UnregisterDeviceRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}