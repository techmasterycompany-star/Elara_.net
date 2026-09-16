using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;

namespace Elara.Application.Interfaces.Service
{
    public interface IShipmentService
    {
        Task<PaginatedResponse<AdminShipmentListDto>> GetAllShipmentsAsync(AdminShipmentFilterDto shipmentRequest);
        Task<AdminShipmentDetailsDto> GetShipmentByIdAsync(long shipmentId);
        Task UpdateShipmentStatusAsync(long shipmentId, UpdateShipmentStatusRequestDto updateRequest);
        Task UpdateShipmentAsync(long shipmentId, AdminUpdateShipmentRequestDto request);
    }
}
