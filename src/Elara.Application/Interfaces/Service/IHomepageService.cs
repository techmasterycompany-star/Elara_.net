using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;

namespace Elara.Application.Interfaces.Service
{
    public interface IHomepageService
    {
        Task<HomepageDto> GetHomepageAsync();
        Task<IEnumerable<BannerDto>> GetActiveBannersAsync();
        Task<IEnumerable<HomepageSectionDto>> GetActiveSectionsAsync();
        Task<PaginatedResponse<BannerDto>> GetAdminBannersAsync(AdminBannerQuery query);
        Task<PaginatedResponse<HomepageSectionDto>> GetAdminSectionsAsync(HomepageSectionQuery query);
        Task<BannerDto> CreateBannerAsync(CreateBannerDto dto);
        Task<BannerDto> UpdateBannerAsync(long bannerId, UpdateBannerDto dto);
        Task UpdateBannerStatusAsync(long bannerId, bool isActive);
        Task DeleteBannerAsync(long bannerId);

        Task<HomepageSectionDto> CreateSectionAsync(CreateHomepageSectionDto dto);
        Task<HomepageSectionDto> UpdateSectionAsync(long sectionId, UpdateHomepageSectionDto dto);
        Task DeleteSectionAsync(long sectionId);
    }


}
