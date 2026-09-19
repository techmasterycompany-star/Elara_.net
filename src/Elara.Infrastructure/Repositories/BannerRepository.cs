using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.HomePageContent;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _context;

        public BannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationQueryResult<Banner>> GetAdminBannersAsync(AdminBannerQuery query)
        {
            var banners = _context.Banners.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
                banners = banners.Where(b => b.Title.Contains(query.Search) || b.Subtitle.Contains(query.Search));

            if (query.Position.HasValue)
                banners = banners.Where(b => b.Position == query.Position.Value);

            if (query.IsActive.HasValue)
                banners = banners.Where(b => b.IsActive == query.IsActive.Value);

            var desc = query.SortOrder == SortOrderEnum.Desc;

            banners = query.SortBy?.ToLower() switch
            {
                "title" => desc ? banners.OrderByDescending(b => b.Title) : banners.OrderBy(b => b.Title),
                "position" => desc ? banners.OrderByDescending(b => b.Position) : banners.OrderBy(b => b.Position),
                "displayorder" => desc ? banners.OrderByDescending(b => b.DisplayOrder) : banners.OrderBy(b => b.DisplayOrder),
                "startdate" => desc ? banners.OrderByDescending(b => b.StartDate) : banners.OrderBy(b => b.StartDate),
                "enddate" => desc ? banners.OrderByDescending(b => b.EndDate) : banners.OrderBy(b => b.EndDate),
                _ => desc ? banners.OrderByDescending(b => b.CreatedAt) : banners.OrderBy(b => b.CreatedAt)
            };

            var totalCount = await banners.CountAsync();

            var items = await banners
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Banner>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<Banner>> GetActiveAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive &&
                    (!b.StartDate.HasValue || b.StartDate <= now) &&
                    (!b.EndDate.HasValue || b.EndDate >= now))
                .OrderBy(b => b.Position)
                .ThenBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        public Task<Banner?> GetByIdAsync(long bannerId)
        {
            return _context.Banners.FirstOrDefaultAsync(x => x.Id == bannerId);
        }

        public async Task<Banner> AddAsync(Banner banner)
        {
            await _context.Banners.AddAsync(banner);
            return banner;
        }

        public Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Banner banner)
        {
            _context.Banners.Remove(banner);
            return Task.CompletedTask;
        }
    }
}
