using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(GetUsersRequest request)
        {
            var query = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (request.IsDeleted.HasValue)
            {
                query = query.Where(u => u.IsDeleted == request.IsDeleted.Value);
            }

            if (request.EmailConfirmed.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == request.EmailConfirmed.Value);
            }

            if (request.Role.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == (long)request.Role.Value));
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search) || u.PhoneNumber.Contains(request.Search) || u.Username.Contains(request.Search));
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == userId);
        }
        public Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChangesAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(long roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<long> roleIds)
        {
            return await _context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
