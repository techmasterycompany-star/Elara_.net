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
        Task<PaginatedResponse<SellerShipmentListDto>> GetSellerShipmentsAsync(long userId, SellerShipmentQuery query);
        Task<SellerShipmentDetailsDto> GetSellerShipmentByIdAsync(long shipmentId, long userId);
        Task<SellerShipmentDetailsDto> CreateSellerShipmentAsync(long orderId, long userId, CreateSellerShipmentDto dto);
        Task UpdateSellerShipmentAsync(long shipmentId, long userId, UpdateSellerShipmentDto dto);
        Task<IEnumerable<CustomerShipmentListDto>> GetCustomerShipmentsByOrderIdAsync(long orderId, long userId);
        Task<CustomerShipmentDetailsDto> GetCustomerShipmentByIdAsync(long orderId, long shipmentId, long userId);
    }
}
