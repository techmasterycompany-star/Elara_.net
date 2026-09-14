using AutoMapper;
using Elara.Application.DTOs.User;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserListDto>();

            CreateMap<User, UserDetailsDto>()
                .ForMember(
                    dest => dest.Roles,
                    opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name))
                );

            CreateMap<User, UserProfileDto>()
                .ForMember(
                    dest => dest.Roles,
                    opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name))
                );

            CreateMap<Role, RoleDto>();
        }
    }
}
