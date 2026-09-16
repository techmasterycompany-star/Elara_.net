using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(AdminOrderFilterDto request)
        {
            var query = _context.Orders
                   .AsNoTracking()
                   .Include(o => o.User)
                   .Include(o => o.Items).ThenInclude(i => i.Product)
                   .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    (o.User != null && (o.User.FullName.Contains(search) || o.User.Email.Contains(search))) ||
                    (!string.IsNullOrEmpty(o.GuestFullName) &&  o.GuestFullName.Contains(search)) ||
                    (!string.IsNullOrEmpty(o.GuestEmail) &&  o.GuestEmail.Contains(search)) ||
                    (!string.IsNullOrEmpty(o.GuestPhoneNumber) &&  o.GuestPhoneNumber.Contains(search)));
            }

            // Status
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            // Date range
            if (request.FromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= request.ToDate.Value);
            }

            // Customer
            if (request.CustomerId.HasValue)
            {
                query = query.Where(o => o.UserId == request.CustomerId.Value);
            }

            // Seller
            if (request.SellerId.HasValue)
            {
                query = query.Where(o => o.Items.Any(i => i.Product.SellerProfileId == request.SellerId.Value));
            }

            // Sorting
            query = ApplySorting(query, request);

            // Pagination
            var skip = (request.PageNumber - 1) * request.Limit;

            return await query
                .Skip(skip)
                .Take(request.Limit)
                .ToListAsync();
        }

        private static IQueryable<Order> ApplySorting(IQueryable<Order> query, AdminOrderFilterDto request)
        {
            var descending = request.SortOrder == SortOrderEnum.Desc;

            return request.SortBy switch
            {
                OrderSortBy.Id => descending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),

                OrderSortBy.OrderDate => descending? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),

                OrderSortBy.TotalAmount => descending? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),

                OrderSortBy.Status => descending? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),

                _ => query.OrderByDescending(o => o.OrderDate)
            };
        }

        public async Task<Order?> GetOrderByIdAsync(long id)
        {
            return await _context.Orders.Include(o => o.Items)
                .Include(o => o.Shipments)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<Order?> GetOrderDetailsAsync(long id)
        {
            return await _context.Orders
                .AsSplitQuery()
                .Include(o => o.User)
                .Include(o => o.ShippingMethod)
                .Include(o => o.PromoCode)
                .Include(o => o.Payment)
                .Include(o => o.StatusHistory)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.SellerProfile)
                        .ThenInclude(sp => sp.User)
                .Include(o => o.Shipments)
                    .ThenInclude(s => s.SellerProfile)
                        .ThenInclude(sp => sp.User)
                .Include(o => o.Shipments)
                    .ThenInclude(s => s.Items)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
