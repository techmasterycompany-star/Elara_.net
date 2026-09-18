using AutoMapper;
using Elara.Application.DTOs.SellerApplication;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class SellerApplicationProfile : Profile
    {
        public SellerApplicationProfile()
        {
            CreateMap<SellerApplication, SellerApplicationListDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email));

            CreateMap<SellerApplication, SellerApplicationDetailsDto>()
                .ForMember(
                    dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src => src.User.Username))
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(
                    dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber));
        }
    }
}
