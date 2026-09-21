using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context) => this.context = context;


        public async Task<PaginationQueryResult<User>> GetAllUsersAsync(GetUsersRequest request)
        {
            var query = context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            if (request.IsDeleted.HasValue)
                query = query.Where(u => u.IsDeleted == request.IsDeleted.Value);

            if (request.EmailConfirmed.HasValue)
                query = query.Where(u => u.EmailConfirmed == request.EmailConfirmed.Value);

            if (request.Role.HasValue)
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == (long)request.Role.Value));

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search) || u.PhoneNumber.Contains(request.Search) || u.Username.Contains(request.Search));

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<User>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
            await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        public async Task<User?> GetByIdAsync(long id) =>
            await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByIdWithAddressesAsync(long id) =>
            await context.Users.Include(u => u.Addresses).Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<Role?> GetRoleByIdAsync(long roleId)
        {
            return await context.Roles.FindAsync(roleId);
        }

        public async Task<IEnumerable<Role>> GetRolesByIdsAsync(IEnumerable<long> roleIds)
        {
            return await context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
