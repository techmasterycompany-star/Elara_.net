using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
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
            return await _context.HomepageSections
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Include(s => s.Banner)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public Task<HomepageSection?> GetByIdAsync(long sectionId)
        {
            return _context.HomepageSections.FirstOrDefaultAsync(x => x.Id == sectionId);
        }

        public async Task<PaginationQueryResult<HomepageSection>> GetSectionsAsync(HomepageSectionQuery query)
        {
            var sections = _context.HomepageSections
                .AsNoTracking()
                .Include(s => s.Banner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                sections = sections.Where(s =>
                    s.Title.Contains(query.Search) ||
                    (s.SubTitle != null && s.SubTitle.Contains(query.Search)));
            }

            if (query.Type.HasValue)
                sections = sections.Where(s => s.Type == query.Type.Value);

            if (query.IsActive.HasValue)
                sections = sections.Where(s => s.IsActive == query.IsActive.Value);

            var desc = query.SortOrder == SortOrderEnum.Desc;

            sections = query.SortBy switch
            {
                HomepageSectionSortBy.Title => desc ? sections.OrderByDescending(s => s.Title) : sections.OrderBy(s => s.Title),
                HomepageSectionSortBy.Type => desc ? sections.OrderByDescending(s => s.Type) : sections.OrderBy(s => s.Type),
                HomepageSectionSortBy.DisplayOrder => desc ? sections.OrderByDescending(s => s.DisplayOrder) : sections.OrderBy(s => s.DisplayOrder),
                HomepageSectionSortBy.MaxItems => desc ? sections.OrderByDescending(s => s.MaxItems) : sections.OrderBy(s => s.MaxItems),
                HomepageSectionSortBy.UpdatedAt => desc ? sections.OrderByDescending(s => s.UpdatedAt) : sections.OrderBy(s => s.UpdatedAt),
                _ => desc ? sections.OrderByDescending(s => s.CreatedAt) : sections.OrderBy(s => s.CreatedAt)
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
