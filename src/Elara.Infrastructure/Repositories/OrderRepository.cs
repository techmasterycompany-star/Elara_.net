using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
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

        public async Task<PaginationQueryResult<Order>> GetSellerOrdersAsync(long sellerProfileId, SellerOrderQuery query)
        {
            var orders = _context.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.Items.Any(i => i.Product.SellerProfileId == sellerProfileId) && o.Status != OrderStatus.Pending)
                .Include(o => o.Items.Where(i => i.Product.SellerProfileId == sellerProfileId))
                    .ThenInclude(i => i.Product)
                .AsQueryable();

            if (query.Status.HasValue)
                orders = orders.Where(o => o.Status == query.Status.Value);

            if (query.DateFrom.HasValue)
                orders = orders.Where(o => o.OrderDate >= query.DateFrom.Value);

            if (query.DateTo.HasValue)
            {
                var toDate = query.DateTo.Value.Date.AddDays(1);
                orders = orders.Where(o => o.OrderDate < toDate);
            }

            orders = query.SortOrder == SortOrderEnum.Desc
                ? orders.OrderByDescending(o => o.OrderDate)
                : orders.OrderBy(o => o.OrderDate);

            var totalCount = await orders.CountAsync();

            var items = await orders
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Order>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Order?> GetSellerOrderDetailsAsync(long orderId, long sellerProfileId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items.Where(i => i.Product.SellerProfileId == sellerProfileId))
                    .ThenInclude(i => i.Product)
                .Include(o => o.Items.Where(i => i.Product.SellerProfileId == sellerProfileId))
                    .ThenInclude(i => i.ShipmentItems)
                        .ThenInclude(si => si.Shipment)
                .FirstOrDefaultAsync(o =>
                    !o.IsDeleted &&
                    o.Id == orderId &&
                    o.Items.Any(i => i.Product.SellerProfileId == sellerProfileId));
        }

        public async Task<PaginationQueryResult<Order>> GetAllOrdersAsync(AdminOrderFilterDto request)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted)
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    (o.User != null && (o.User.FullName.Contains(search) || o.User.Email.Contains(search))) ||
                    (!string.IsNullOrEmpty(o.GuestFullName) && o.GuestFullName.Contains(search)) ||
                    (!string.IsNullOrEmpty(o.GuestEmail) && o.GuestEmail.Contains(search)) ||
                    (!string.IsNullOrEmpty(o.GuestPhoneNumber) && o.GuestPhoneNumber.Contains(search)));
            }

            if (request.Status.HasValue)
                query = query.Where(o => o.Status == request.Status.Value);

            if (request.FromDate.HasValue)
                query = query.Where(o => o.OrderDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < toDate);
            }

            if (request.CustomerId.HasValue)
                query = query.Where(o => o.UserId == request.CustomerId.Value);

            if (request.SellerId.HasValue)
                query = query.Where(o => o.Items.Any(i => i.Product.SellerProfileId == request.SellerId.Value));

            query = ApplySorting(query, request);

            var totalCount = await query.CountAsync();
            var skip = (request.PageNumber - 1) * request.Limit;

            var items = await query
                .Skip(skip)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Order>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Order> ApplySorting(IQueryable<Order> query, AdminOrderFilterDto request)
        {
            var descending = request.SortOrder == SortOrderEnum.Desc;

            return request.SortBy switch
            {
                OrderSortBy.Id => descending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),
                OrderSortBy.OrderDate => descending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                OrderSortBy.TotalAmount => descending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                OrderSortBy.Status => descending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                _ => query.OrderByDescending(o => o.OrderDate)
            };
        }

        public async Task<Order?> GetOrderByIdAsync(long id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.Shipments)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == id);
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
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == id);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationQueryResult<Order>> GetCustomerOrdersAsync(long userId, GetMyOrdersRequest request)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.UserId == userId)
                .Include(o => o.Items)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.PageNumber - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Order>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Order?> GetCustomerOrderDetailsAsync(long orderId, long userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Shipments)
                    .ThenInclude(s => s.Items)
                        .ThenInclude(si => si.OrderItem)
                .Include(o => o.StatusHistory)
                .Include(o => o.Payment)
                .Include(o => o.ShippingMethod)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId && o.UserId == userId);
        }

        public async Task<Order?> GetCustomerOrderForUpdateAsync(long orderId, long userId)
        {
            return await _context.Orders
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == orderId && o.UserId == userId);
        }

        public async Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(long orderId, long userId)
        {
            return await _context.OrderStatusHistories
                .AsNoTracking()
                .Where(h => h.OrderId == orderId && h.Order.UserId == userId && !h.Order.IsDeleted)
                .OrderBy(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(long userId, long productId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId 
                    && o.Status == Elara.Domain.Enums.OrderStatus.Delivered
                    && o.Items.Any(i => i.ProductId == productId)
                    && !o.IsDeleted)
                .AnyAsync();
        }
    }
}
