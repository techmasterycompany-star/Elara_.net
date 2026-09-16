using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository.Auth
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(long id);
        Task<List<Role>> GetAllAsync();
    }
}
