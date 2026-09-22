using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoleEnum = Elara.Domain.Enums.Role;

namespace Elara.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("/api/v1/admin/roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(roles));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest request)
        {
            var users = await _userService.GetAllUsersAsync(request);
            return Ok(ApiResponse<PaginatedResponse<UserListDto>>.SuccessResponse(users));
        }

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetUserById(long userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(ApiResponse<UserDetailsDto>.SuccessResponse(user));
        }

        [HttpDelete("{userId:long}")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            await _userService.DeleteUser(userId);
            return Ok(ApiResponse<string>.SuccessResponse("User deleted successfully."));
        }

        [HttpPut("{userId:long}/roles")]
        public async Task<IActionResult> AssignRolesToUser(long userId, [FromBody] List<RoleEnum> roles)
        {
            await _userService.AssignRolesToUserAsync(userId, roles);
            return Ok(ApiResponse<string>.SuccessResponse("Roles assigned successfully."));
        }

        [HttpPatch("{userId:long}/suspend")]
        public async Task<IActionResult> SuspendUser(long userId)
        {
            await _userService.UpdateUserStatus(userId, isActive:false);

            return Ok(ApiResponse<string>.SuccessResponse("User suspended successfully."));
        }

        [HttpPatch("{userId:long}/unsuspend")]
        public async Task<IActionResult> UnsuspendUser(long userId)
        {
            await _userService.UpdateUserStatus(userId, isActive: true);

            return Ok(ApiResponse<string>.SuccessResponse("User unsuspended successfully."));
        }


    }
}
