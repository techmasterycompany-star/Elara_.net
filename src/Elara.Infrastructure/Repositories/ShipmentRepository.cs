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

        public async Task<IEnumerable<Shipment>> GetAllShipmentsAsync(AdminShipmentFilterDto request)
        {
            var query = _context.Shipments
                   .AsNoTracking()
                   .Include(o => o.SellerProfile).ThenInclude(sp => sp.User)
                   .Include(o => o.Items).ThenInclude(si => si.OrderItem)
                   .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(s => s.Carrier.Contains(search) ||
                                         s.TrackingNumber.Contains(search) ||
                                         s.SellerProfile.User.FullName.Contains(search) ||
                                         s.Items.Any(i => i.OrderItem.Product.Name.Contains(search)));
            }

            // Status
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            // Date range
            if (request.FromDate.HasValue)
            {
                query = query.Where(o => o.ShippedDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(o => o.ShippedDate <= request.ToDate.Value);
            }

            // Customer
            if (request.CustomerId.HasValue)
            {
                query = query.Where(o => o.Order.UserId == request.CustomerId.Value);
            }

            // Seller
            if (request.SellerId.HasValue)
            {
                query = query.Where(o => o.SellerProfileId == request.SellerId.Value);
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
                .Include(o => o.Items).ThenInclude(s => s.OrderItem).ThenInclude(oi => oi.Product)
                .Include(o => o.SellerProfile).ThenInclude(sp => sp.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateShipmentAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Shipment>> GetShipmentsByOrderIdAsync(long orderId)
        {
            return await _context.Shipments
                .Where(s => s.OrderId == orderId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
