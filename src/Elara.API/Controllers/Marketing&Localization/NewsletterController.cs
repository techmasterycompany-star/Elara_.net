using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v1/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly INewsletterService _newsletterService;

        public NewsletterController(INewsletterService newsletterService)
        {
            _newsletterService = newsletterService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var result = await _newsletterService.SubscribeAsync(request.Email, request.UserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
        {
            var result = await _newsletterService.UnsubscribeAsync(request.Email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("subscribers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubscribers()
        {
            var result = await _newsletterService.GetSubscribersAsync();
            return Ok(result);
        }

        [HttpPost("campaigns/send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendCampaign([FromBody] SendCampaignRequestDto request)
        {
            var result = await _newsletterService.SendCampaignAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }

    public class SubscribeRequest
    {
        public string Email { get; set; } = string.Empty;
        public long? UserId { get; set; }
    }

    public class UnsubscribeRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}