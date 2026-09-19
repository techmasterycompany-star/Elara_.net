using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IHomepageSectionRepository
    {
        Task<IEnumerable<HomepageSection>> GetActiveAsync();
        Task<PaginationQueryResult<HomepageSection>> GetSectionsAsync(HomepageSectionQuery query);
        Task<HomepageSection?> GetByIdAsync(long sectionId);
        Task<HomepageSection> AddAsync(HomepageSection section);
        Task UpdateAsync(HomepageSection section);
        Task DeleteAsync(HomepageSection section);
        Task SaveChangesAsync();
    }
}
