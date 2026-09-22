using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IBannerRepository
    {
        Task<PaginationQueryResult<Banner>> GetAdminBannersAsync(AdminBannerQuery query);
        Task<Banner?> GetByIdAsync(long bannerId);
        Task<IEnumerable<Banner>> GetActiveAsync();
        Task<Banner> AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(Banner banner);
        Task SaveChangesAsync();
    }
}
