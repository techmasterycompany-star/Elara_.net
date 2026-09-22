using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using RoleEnum = Elara.Domain.Enums.Role;

namespace Elara.Application.Interfaces.Service
{
    public interface IUserService
    {
        Task<PaginatedResponse<UserListDto>> GetAllUsersAsync(GetUsersRequest request);
        Task<UserDetailsDto?> GetUserByIdAsync(long userId);
        Task<UserProfileDto?> GetUserProfileAsync(long userId);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<bool> AssignRolesToUserAsync(long userId, List<RoleEnum> roles);
        Task<bool> UpdateUserStatus(long userId, bool isActive);
        Task<bool> DeleteUser(long userId);
    }
}
