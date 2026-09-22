using AutoMapper;
using Elara.Application.DTOs.SellerProfile;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
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