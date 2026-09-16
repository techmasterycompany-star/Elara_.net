using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoleEnum = Elara.Domain.Enums.Role;

namespace Elara.API.Controllers.Admin
{
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
            return Ok(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest request)
        {
            var users = await _userService.GetAllUsersAsync(request);
            return Ok(users);
        }

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetUserById(long userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpPut("{userId:long}/status")]
        public async Task<IActionResult> UpdateUserStatus(long userId, [FromBody] UpdateUserStatusDto updateUserStatusDto)
        {
            await _userService.UpdateUserStatus(userId, updateUserStatusDto.IsActive);
            return NoContent();
        }

        [HttpDelete("{userId:long}")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            await _userService.DeleteUser(userId);
            return NoContent();
        }

        [HttpPut("{userId:long}/roles")]
        public async Task<IActionResult> AssignRolesToUser(long userId, [FromBody] List<RoleEnum> roles)
        {
            await _userService.AssignRolesToUserAsync(userId, roles);
            return NoContent();
        }
    }
}
