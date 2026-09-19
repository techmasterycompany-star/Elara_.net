using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.DTOs.SellerProfile;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elara.API.Controllers.Seller
{
    [ApiController]
    [Route("api/sellers")]
    public class SellersController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellersController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply(ApplyAsSellerDto request)
        {
            var userId = User.GetAuthenticatedUserId();

            await _sellerService.ApplyAsync(userId, request);

            return Ok(ApiResponse<string>.SuccessResponse("Application submitted successfully."));
        }

        [HttpGet("me/application")]
        [Authorize]
        public async Task<IActionResult> GetMyApplication()
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _sellerService.GetMyApplicationAsync(userId);

            return Ok(ApiResponse<SellerApplicationDto>.SuccessResponse(result));
        }

        [HttpDelete("me/application")]
        [Authorize]
        public async Task<IActionResult> WithdrawApplication()
        {
            var userId = User.GetAuthenticatedUserId();

            await _sellerService.WithdrawApplicationAsync(userId);

            return Ok(ApiResponse<string>.SuccessResponse("Seller application withdrawn successfully."));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetAuthenticatedUserId();

            var result = await _sellerService.GetMyProfileAsync(userId);

            return Ok(ApiResponse<SellerProfileDto>.SuccessResponse(result));
        }

        [HttpPut("me")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateMyProfile(UpdateSellerProfileDto request)
        {
            var userId = User.GetAuthenticatedUserId();

            await _sellerService.UpdateMyProfileAsync(userId, request);

            return Ok(ApiResponse<string>.SuccessResponse("Seller profile updated successfully."));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSellers([FromQuery] GetSellersRequest request)
        {
            var result = await _sellerService.GetSellersAsync(request);

            return Ok(ApiResponse<PaginatedResponse<SellerListDto>>.SuccessResponse(result));
        }

        [HttpGet("{sellerId:long}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeller(long sellerId)
        {
            var result = await _sellerService.GetSellerByIdAsync(sellerId);

            return Ok(ApiResponse<SellerListDto>.SuccessResponse(result));
        }

        [HttpGet("{sellerId:long}/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSellerProducts(long sellerId, [FromQuery] GetSellerProductsRequest request)
        {
            var result = await _sellerService.GetSellerProductsAsync(sellerId, request);

            return Ok(ApiResponse<PaginatedResponse<SellerProductDto>>.SuccessResponse(result));
        }
    }
}
