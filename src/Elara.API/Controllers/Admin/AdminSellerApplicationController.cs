using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/seller-applications")]
    [Authorize(Roles = "Admin")]
    public class AdminSellerApplicationsController : ControllerBase
    {
        private readonly ISellerApplicationService _sellerApplicationService;

        public AdminSellerApplicationsController(ISellerApplicationService sellerApplicationService)
        {
            _sellerApplicationService = sellerApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery] GetSellerApplicationsRequest request)
        {
            var result = await _sellerApplicationService.GetApplicationsAsync(request);

            return Ok(ApiResponse<PaginatedResponse<SellerApplicationListDto>>.SuccessResponse(result));
        }

        [HttpGet("{applicationId:long}")]
        public async Task<IActionResult> GetApplication(long applicationId)
        {
            var result = await _sellerApplicationService.GetApplicationByIdAsync(applicationId);

            return Ok(ApiResponse<SellerApplicationDetailsDto>.SuccessResponse(result));
        }

        [HttpPatch("{applicationId:long}/approve")]
        public async Task<IActionResult> ApproveApplication(long applicationId)
        {
            await _sellerApplicationService.ApproveApplicationAsync(applicationId);
            return Ok(ApiResponse<string>.SuccessResponse("Application approved successfully."));
        }

        [HttpPatch("{applicationId:long}/reject")]
        public async Task<IActionResult> RejectApplication(long applicationId,[FromBody] RejectSellerApplicationDto request)
        {
            await _sellerApplicationService.RejectApplicationAsync(applicationId, request);
            return Ok(ApiResponse<string>.SuccessResponse("Application rejected successfully."));
        }
    }
}
