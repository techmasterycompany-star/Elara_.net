using Elara.Application.DTOs.Shipment;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IShipmentRepository 
    { 
        Task<IEnumerable<Shipment>> GetAllShipmentsAsync(AdminShipmentFilterDto shipmentRequest);
        Task<Shipment?> GetShipmentByIdAsync(long id);
        Task UpdateShipmentAsync(Shipment shipment);
        Task<List<Shipment>> GetShipmentsByOrderIdAsync(long orderId);
        Task SaveChangesAsync();
    }
}
