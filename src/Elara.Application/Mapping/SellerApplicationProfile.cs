using AutoMapper;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.DTOs.SellerProfile;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class SellerApplicationMappingProfile : Profile
    {
        public SellerApplicationMappingProfile()
        {
            CreateMap<SellerApplication, SellerApplicationListDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email));

            CreateMap<ApplyAsSellerDto, SellerApplication>();
            CreateMap<SellerApplication, SellerApplicationDto>();

            CreateMap<SellerApplication, SellerApplicationDetailsDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User.PhoneNumber));
        }
    }

    public class SellerProfileMappingProfile : Profile
    {
        public SellerProfileMappingProfile()
        {
            CreateMap<UpdateSellerProfileDto, SellerProfile>();
            CreateMap<SellerProfile, SellerProfileDto>();
            CreateMap<SellerProfile, SellerListDto>();

            CreateMap<Product, SellerProductDto>()
                .ForMember(dest => dest.MainImageUrl,
                    opt => opt.MapFrom(src => src.Images
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()));
        }
    }
}