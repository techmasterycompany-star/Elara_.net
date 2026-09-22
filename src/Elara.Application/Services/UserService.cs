using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.User;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using RoleEnum = Elara.Domain.Enums.Role;

namespace Elara.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<PaginatedResponse<UserListDto>> GetAllUsersAsync(GetUsersRequest request)
        {
            var users = await _userRepository.GetAllUsersAsync(request);
            try { 
            
            var userDtos = _mapper.Map<IEnumerable<UserListDto>>(users.Items).ToList();
            return new PaginatedResponse<UserListDto>
            {
                Data = userDtos,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = users.TotalCount,
                TotalPages = (int)Math.Ceiling((double)users.TotalCount / request.Limit)
            };
            }
            catch (AutoMapperMappingException ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");

            return _mapper.Map<UserDetailsDto>(user);
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _userRepository.GetAllRolesAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }


        public async Task<bool> UpdateUserStatus(long userId, bool isActive)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }


        public async Task<bool> AssignRolesToUserAsync(long userId, List<RoleEnum> roles)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var roleIds = roles
                .Distinct()
                .Select(role => (long)role)
                .ToList();

            var dbRoles = await _userRepository.GetRolesByIdsAsync(roleIds);

            if (dbRoles.Count() != roleIds.Count)
                throw new NotFoundException("One or more role values are invalid.");

            user.UserRoles.Clear();

            foreach (var roleId in roleIds)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

            return true;
        }

        public async Task<bool> DeleteUser(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) throw new NotFoundException("User not found");

            user.IsDeleted = true;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

    }
}
