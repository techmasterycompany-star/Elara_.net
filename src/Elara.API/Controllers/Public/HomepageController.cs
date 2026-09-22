using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Public
{
    [ApiController]
    [Route("api/v1")]
    public class HomepageController : ControllerBase
    {
        private readonly IHomepageService _homepageService;

        public HomepageController(IHomepageService homepageService)
        {
            _homepageService = homepageService;
        }

        [HttpGet("homepage")]
        public async Task<IActionResult> GetHomepage()
        {
            var result = await _homepageService.GetHomepageAsync();

            return Ok(ApiResponse<HomepageDto>.SuccessResponse(result));
        }

        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners()
        {
            var result = await _homepageService.GetActiveBannersAsync();

            return Ok(ApiResponse<IEnumerable<BannerDto>>.SuccessResponse(result));
        }

        [HttpGet("homepage/sections")]
        public async Task<IActionResult> GetSections()
        {
            var result = await _homepageService.GetActiveSectionsAsync();

            return Ok(ApiResponse<IEnumerable<HomepageSectionDto>>.SuccessResponse(result));
        }
    }
}
