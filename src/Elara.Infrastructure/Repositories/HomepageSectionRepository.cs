using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class HomepageSectionRepository : IHomepageSectionRepository
    {
        private readonly AppDbContext _context;

        public HomepageSectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HomepageSection>> GetActiveAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.HomepageSections
                .AsNoTracking()
                .Include(s => s.Banner)
                .Where(s =>
                    s.IsActive &&
                    (
                        s.Type != HomepageSectionType.Banner ||
                        (
                            s.Banner != null &&
                            s.Banner.IsActive &&
                            (!s.Banner.StartDate.HasValue || s.Banner.StartDate <= now) &&
                            (!s.Banner.EndDate.HasValue || s.Banner.EndDate >= now)
                        )
                    ))
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Id)
                .ToListAsync();
        }

        public Task<HomepageSection?> GetByIdAsync(long sectionId)
        {
            return _context.HomepageSections
                .FirstOrDefaultAsync(s => s.Id == sectionId);
        }

        public async Task<PaginationQueryResult<HomepageSection>> GetSectionsAsync(HomepageSectionQuery query)
        {
            var sections = _context.HomepageSections
                .AsNoTracking()
                .Include(s => s.Banner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                sections = sections.Where(s =>
                    s.Title.Contains(search) ||
                    (s.SubTitle != null && s.SubTitle.Contains(search)));
            }

            if (query.Type.HasValue)
                sections = sections.Where(s => s.Type == query.Type.Value);

            if (query.IsActive.HasValue)
                sections = sections.Where(s => s.IsActive == query.IsActive.Value);

            var desc = query.SortOrder == SortOrderEnum.Desc;

            sections = query.SortBy switch
            {
                HomepageSectionSortBy.Title => desc
                    ? sections.OrderByDescending(s => s.Title).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.Title).ThenBy(s => s.Id),

                HomepageSectionSortBy.Type => desc
                    ? sections.OrderByDescending(s => s.Type).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.Type).ThenBy(s => s.Id),

                HomepageSectionSortBy.DisplayOrder => desc
                    ? sections.OrderByDescending(s => s.DisplayOrder).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Id),

                HomepageSectionSortBy.MaxItems => desc
                    ? sections.OrderByDescending(s => s.MaxItems).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.MaxItems).ThenBy(s => s.Id),

                HomepageSectionSortBy.UpdatedAt => desc
                    ? sections.OrderByDescending(s => s.UpdatedAt).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.UpdatedAt).ThenBy(s => s.Id),

                _ => desc
                    ? sections.OrderByDescending(s => s.CreatedAt).ThenByDescending(s => s.Id)
                    : sections.OrderBy(s => s.CreatedAt).ThenBy(s => s.Id)
            };

            var totalCount = await sections.CountAsync();

            var items = await sections
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PaginationQueryResult<HomepageSection>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<HomepageSection> AddAsync(HomepageSection section)
        {
            await _context.HomepageSections.AddAsync(section);
            return section;
        }

        public Task UpdateAsync(HomepageSection section)
        {
            _context.HomepageSections.Update(section);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(HomepageSection section)
        {
            _context.HomepageSections.Remove(section);
            return Task.CompletedTask;
        }
    }
}
