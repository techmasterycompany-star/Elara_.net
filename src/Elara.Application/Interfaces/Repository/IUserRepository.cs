using Elara.Application.DTOs.User;
using Elara.Domain.Entities;


namespace Elara.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync(GetUsersRequest request);
        Task<User?> GetUserByIdAsync(long userId);
        Task<Role?> GetRoleByIdAsync(long roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<long> roleIds);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
