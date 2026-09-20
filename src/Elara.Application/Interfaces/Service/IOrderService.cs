using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Service
{
    public interface IOrderService
    {
        Task<PaginatedResponse<SellerOrderListDto>> GetSellerOrdersAsync(long userId, SellerOrderQuery query);
        Task<SellerOrderDetailsDto> GetSellerOrderByIdAsync(long orderId, long userId);
        Task<PaginatedResponse<AdminOrderListDto>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest);
        Task<AdminOrderDetailsDto> GetOrderByIdAsync(long orderId);
        Task UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequestDto updateRequest);
    }
}
