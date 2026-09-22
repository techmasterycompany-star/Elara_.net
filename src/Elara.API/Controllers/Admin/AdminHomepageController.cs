using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminHomepageController : ControllerBase
    {
        private readonly IHomepageService _homepageService;

        public AdminHomepageController(IHomepageService homepageService)
        {
            _homepageService = homepageService;
        }

        [HttpGet("banners")]
        public async Task<IActionResult> GetBanners([FromQuery] AdminBannerQuery query)
        {
            var result = await _homepageService.GetAdminBannersAsync(query);

            return Ok(ApiResponse<PaginatedResponse<BannerDto>>.SuccessResponse(result));
        }

        [HttpPost("banners")]
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDto dto)
        {
            var result = await _homepageService.CreateBannerAsync(dto);

            return Ok(ApiResponse<BannerDto>.SuccessResponse(result));
        }

        [HttpPut("banners/{bannerId:long}")]
        public async Task<IActionResult> UpdateBanner(long bannerId, [FromForm] UpdateBannerDto dto)
        {
            var result = await _homepageService.UpdateBannerAsync(bannerId, dto);

            return Ok(ApiResponse<BannerDto>.SuccessResponse(result));
        }

        [HttpPatch("banners/{bannerId:long}/status")]
        public async Task<IActionResult> UpdateBannerStatus(long bannerId, [FromBody] UpdateBannerStatusDto dto)
        {
            await _homepageService.UpdateBannerStatusAsync(bannerId, dto.IsActive);

            return Ok(ApiResponse<string>.SuccessResponse("Banner status updated successfully."));
        }

        [HttpDelete("banners/{bannerId:long}")]
        public async Task<IActionResult> DeleteBanner(long bannerId)
        {
            await _homepageService.DeleteBannerAsync(bannerId);

            return Ok(ApiResponse<string>.SuccessResponse("Banner deleted successfully."));
        }

        [HttpGet("homepage/sections")]
        public async Task<IActionResult> GetSections([FromQuery] HomepageSectionQuery query)
        {
            var result = await _homepageService.GetAdminSectionsAsync(query);

            return Ok(ApiResponse<PaginatedResponse<HomepageSectionDto>>.SuccessResponse(result));
        }

        [HttpPost("homepage/sections")]
        public async Task<IActionResult> CreateSection([FromBody] CreateHomepageSectionDto dto)
        {
            var result = await _homepageService.CreateSectionAsync(dto);

            return StatusCode(201, ApiResponse<HomepageSectionDto>.SuccessResponse(result));
        }

        [HttpPut("homepage/sections/{sectionId:long}")]
        public async Task<IActionResult> UpdateSection(long sectionId, [FromBody] UpdateHomepageSectionDto dto)
        {
            var result = await _homepageService.UpdateSectionAsync(sectionId, dto);

            return Ok(ApiResponse<HomepageSectionDto>.SuccessResponse(result));
        }

        [HttpDelete("homepage/sections/{sectionId:long}")]
        public async Task<IActionResult> DeleteSection(long sectionId)
        {
            await _homepageService.DeleteSectionAsync(sectionId);

            return Ok(ApiResponse<string>.SuccessResponse("Homepage section deleted successfully."));
        }
    }
}