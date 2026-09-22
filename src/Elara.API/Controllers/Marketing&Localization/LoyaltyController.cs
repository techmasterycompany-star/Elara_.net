using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v1/loyalty")]
    [Authorize]
    public class LoyaltyController : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService;

        public LoyaltyController(ILoyaltyService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = GetCurrentUserId();
            var result = await _loyaltyService.GetBalanceAsync(userId);
            return Ok(result);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = GetCurrentUserId();
            var result = await _loyaltyService.GetTransactionsAsync(userId);
            return Ok(result);
        }

        [HttpPost("redeem")]
        public async Task<IActionResult> Redeem([FromBody] RedeemPointsRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _loyaltyService.RedeemAsync(userId, request.Points);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.Parse(claim!);
        }
    }

    public class RedeemPointsRequest
    {
        public int Points { get; set; }
    }
}