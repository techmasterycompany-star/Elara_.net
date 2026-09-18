using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<PaginationQueryResult<User>> GetAllUsersAsync(GetUsersRequest request);
        Task<User?> GetUserByIdAsync(long userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByIdWithAddressesAsync(long id);
        Task<Role?> GetRoleByIdAsync(long roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<long> roleIds);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
