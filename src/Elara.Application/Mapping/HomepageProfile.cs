using AutoMapper;
using Elara.Application.DTOs.HomePageContent;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class HomepageProfile : Profile
    {
        public HomepageProfile()
        {
            CreateMap<Banner, BannerDto>();

            CreateMap<CreateHomepageSectionDto, HomepageSection>();
            CreateMap<UpdateHomepageSectionDto, HomepageSection>();
            CreateMap<HomepageSection, HomepageSectionDto>();
        }
    }
}
