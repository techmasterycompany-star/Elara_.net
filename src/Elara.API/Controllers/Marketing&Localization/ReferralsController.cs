using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v1/referrals")]
    //[Authorize]
    public class ReferralsController : ControllerBase
    {
        private readonly IReferralService _referralService;

        public ReferralsController(IReferralService referralService)
        {
            _referralService = referralService;
        }

        [HttpGet("my-code")]
        public IActionResult GetMyCode()
        {
            var userId = GetCurrentUserId();
            var result = _referralService.GetMyCode(userId);
            return Ok(result);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyReferralRequest request)
        {
            var referredUserId = GetCurrentUserId();
            var result = await _referralService.ApplyAsync(referredUserId, request.Code);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetCurrentUserId();
            var result = await _referralService.GetHistoryAsync(userId);
            return Ok(result);
        }

        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.Parse(claim!);
        }
    }

    public class ApplyReferralRequest
    {
        public string Code { get; set; } = string.Empty;
    }
}