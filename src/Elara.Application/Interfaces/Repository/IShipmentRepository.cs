using Elara.Application.DTOs.Shipment;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IShipmentRepository 
    { 
        Task<PaginationQueryResult<Shipment>> GetAllShipmentsAsync(AdminShipmentFilterDto shipmentRequest);
        Task<Shipment?> GetShipmentByIdAsync(long id);
        Task UpdateShipmentAsync(Shipment shipment);
        Task<List<Shipment>> GetShipmentsByOrderIdAsync(long orderId);
        Task<PaginationQueryResult<Shipment>> GetSellerShipmentsAsync(long sellerProfileId, SellerShipmentQuery query);
        Task<Shipment?> GetSellerShipmentByIdAsync(long shipmentId, long sellerProfileId);
        Task<Shipment> AddAsync(Shipment shipment);
        Task<List<Shipment>> GetCustomerShipmentsByOrderIdAsync(long orderId, long userId);
        Task<Shipment?> GetCustomerShipmentByIdAsync(long orderId, long shipmentId, long userId);
        Task SaveChangesAsync();
    }
}
