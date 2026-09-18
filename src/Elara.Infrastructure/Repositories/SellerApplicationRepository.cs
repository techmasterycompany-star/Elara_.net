using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.SellerApplication;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Infrastructure.Repositories
{
    public class SellerApplicationRepository : ISellerApplicationRepository
    {
        private readonly AppDbContext _context;

        public SellerApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationQueryResult<SellerApplication>> GetApplicationsAsync(GetSellerApplicationsRequest request)
        {
            var query = _context.SellerApplications
                .AsNoTracking()
                .Include(x => x.User)
                .AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(x => x.Status == request.Status.Value);

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<SellerApplication>
            {
                Items = data,
                TotalCount = totalCount,
            };
        }
        public async Task<SellerApplication?> GetApplicationByIdAsync(long applicationId)
        {
            return await _context.SellerApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == applicationId);
        }


        public async Task<bool> HasPendingApplicationAsync(long userId)
        {
            return await _context.SellerApplications
                .AnyAsync(x => x.UserId == userId && x.Status == SellerApplicationStatus.Pending);
        }

        public async Task UpdateAsync(SellerApplication application)
        {
            _context.SellerApplications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}
