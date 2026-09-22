using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Interfaces.Repository;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _context;

        public ShipmentRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<PaginationQueryResult<Shipment>> GetAllShipmentsAsync(AdminShipmentFilterDto request)
        {
            var query = _context.Shipments
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .Include(s => s.SellerProfile)
                    .ThenInclude(sp => sp.User)
                .Include(s => s.Items)
                    .ThenInclude(si => si.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(s =>
                    s.Carrier.Contains(search) ||
                    s.TrackingNumber.Contains(search) ||
                    s.SellerProfile.User.FullName.Contains(search) ||
                    s.Items.Any(i => i.OrderItem.Product.Name.Contains(search)));
            }

            if (request.Status.HasValue)
                query = query.Where(s => s.Status == request.Status.Value);

            if (request.FromDate.HasValue)
            {
                var fromDate = request.FromDate.Value.Date;
                query = query.Where(s => s.ShippedDate >= fromDate);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(s => s.ShippedDate < toDate);
            }

            if (request.CustomerId.HasValue)
                query = query.Where(s => s.Order.UserId == request.CustomerId.Value);

            if (request.SellerId.HasValue)
                query = query.Where(s => s.SellerProfileId == request.SellerId.Value);

            query = ApplySorting(query, request);

            var totalCount = await query.CountAsync();
            var skip = (request.PageNumber - 1) * request.Limit;

            var items = await query
                .Skip(skip)
                .Take(request.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Shipment>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Shipment> ApplySorting(IQueryable<Shipment> query, AdminShipmentFilterDto request)
        {
            var descending = request.SortOrder == SortOrderEnum.Desc;

            return request.SortBy switch
            {
                ShipmentSortBy.Id => descending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),
                ShipmentSortBy.CreatedAt => descending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
                ShipmentSortBy.ShippedDate => descending ? query.OrderByDescending(o => o.ShippedDate) : query.OrderBy(o => o.ShippedDate),
                ShipmentSortBy.EstimatedDeliveryDate => descending ? query.OrderByDescending(o => o.EstimatedDeliveryDate) : query.OrderBy(o => o.EstimatedDeliveryDate),
                ShipmentSortBy.DeliveredDate => descending ? query.OrderByDescending(o => o.DeliveredDate) : query.OrderBy(o => o.DeliveredDate),
                ShipmentSortBy.Status => descending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                _ => query.OrderByDescending(o => o.CreatedAt)
            };
        }

        public async Task<Shipment?> GetShipmentByIdAsync(long id)
        {
            return await _context.Shipments
                .Include(s => s.Items)
                    .ThenInclude(si => si.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .Include(s => s.SellerProfile)
                    .ThenInclude(sp => sp.User)
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == id);
        }

        public async Task UpdateShipmentAsync(Shipment shipment)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Shipment>> GetShipmentsByOrderIdAsync(long orderId)
        {
            return await _context.Shipments
                .Where(s => !s.IsDeleted && s.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<PaginationQueryResult<Shipment>> GetSellerShipmentsAsync(long sellerProfileId, SellerShipmentQuery query)
        {
            var shipments = _context.Shipments
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.SellerProfileId == sellerProfileId)
                .Include(s => s.Items)
                    .ThenInclude(i => i.OrderItem)
                        .ThenInclude(i => i.Product)
                .AsQueryable();

            if (query.Status.HasValue)
                shipments = shipments.Where(s => s.Status == query.Status.Value);

            if (query.OrderId.HasValue)
                shipments = shipments.Where(s => s.OrderId == query.OrderId.Value);

            shipments = query.SortOrder == SortOrderEnum.Desc
                ? shipments.OrderByDescending(s => s.CreatedAt)
                : shipments.OrderBy(s => s.CreatedAt);

            var totalCount = await shipments.CountAsync();

            var items = await shipments
                .Skip((query.PageNumber - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PaginationQueryResult<Shipment>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Shipment?> GetSellerShipmentByIdAsync(long shipmentId, long sellerProfileId)
        {
            return await _context.Shipments
                .Include(s => s.Items)
                    .ThenInclude(i => i.OrderItem)
                        .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == shipmentId && s.SellerProfileId == sellerProfileId);
        }

        public Task<Shipment> AddAsync(Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            return Task.FromResult(shipment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Shipment>> GetCustomerShipmentsByOrderIdAsync(long orderId, long userId)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Include(s => s.Items)
                    .ThenInclude(i => i.OrderItem)
                .Where(s => !s.IsDeleted && s.OrderId == orderId && s.Order.UserId == userId)
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<Shipment?> GetCustomerShipmentByIdAsync(long orderId, long shipmentId, long userId)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Include(s => s.Items)
                    .ThenInclude(i => i.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.Id == shipmentId && s.OrderId == orderId && s.Order.UserId == userId);
        }
    }
}
