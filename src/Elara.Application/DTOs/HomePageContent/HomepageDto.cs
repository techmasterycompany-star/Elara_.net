namespace Elara.Application.DTOs.HomePageContent
{
    public class HomepageDto
    {
        public IEnumerable<BannerDto> Banners { get; set; } = [];
        public IEnumerable<HomepageSectionDto> Sections { get; set; } = [];
    }
}
