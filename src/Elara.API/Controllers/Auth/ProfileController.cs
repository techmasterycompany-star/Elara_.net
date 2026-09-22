using Elara.API.Helpers;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers.Auth
{
    [Route("api/v1/users/me")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService profileService;
        public ProfileController(IProfileService profileService) => this.profileService = profileService;


        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetAuthenticatedUserId();
            var profile = await profileService.GetProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
           [FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetAuthenticatedUserId();

            var profile = await profileService.UpdateProfileAsync(
                userId,
                request);

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = User.GetAuthenticatedUserId();

            await profileService.DeleteProfileAsync(userId);

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }
        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = User.GetAuthenticatedUserId();

            var addresses = await profileService.GetAddressesAsync(userId);

            return Ok(ApiResponse<IList<AddressDto>>.SuccessResponse(addresses));
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(
            [FromBody] AddAddressRequest request)
        {
            var userId = User.GetAuthenticatedUserId();

            var address = await profileService.AddAddressAsync(
                userId,
                request);

            return Ok(ApiResponse<AddressDto>.SuccessResponse(address));
        }
        [HttpPut("addresses/{addressId:long}")]
        public async Task<IActionResult> UpdateAddress(
           long addressId,
           [FromBody] UpdateAddressRequest request)
        {
            var userId = User.GetAuthenticatedUserId();

            var address = await profileService.UpdateAddressAsync(
                userId,
                addressId,
                request);

            return Ok(ApiResponse<AddressDto>.SuccessResponse(address));
        }

        [HttpDelete("addresses/{addressId:long}")]
        public async Task<IActionResult> DeleteAddress(long addressId)
        {
            var userId = User.GetAuthenticatedUserId();

            await profileService.RemoveAddressAsync(
                userId,
                addressId);

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }
        [HttpPatch("addresses/{addressId:long}/default")]
        public async Task<IActionResult> SetDefaultAddress(long addressId)
        {
            var userId = User.GetAuthenticatedUserId();

            await profileService.SetDefaultAddressAsync(
                userId,
                addressId);

            return Ok(ApiResponse<bool>.SuccessResponse(true));
        }
    }
}
