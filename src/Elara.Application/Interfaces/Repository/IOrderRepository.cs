using Elara.Application.DTOs.Order;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IOrderRepository 
    {
        Task<PaginationQueryResult<Order>> GetSellerOrdersAsync(long sellerProfileId, SellerOrderQuery query);
        Task<Order?> GetSellerOrderDetailsAsync(long orderId, long sellerProfileId);
        Task<PaginationQueryResult<Order>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest);
        Task<Order?> GetOrderByIdAsync(long id);
        Task<Order?> GetOrderDetailsAsync(long id);
        Task UpdateOrderAsync(Order order);
        Task<PaginationQueryResult<Order>> GetCustomerOrdersAsync(long userId, GetMyOrdersRequest request);
        Task<Order?> GetCustomerOrderDetailsAsync(long orderId, long userId);
        Task<Order?> GetCustomerOrderForUpdateAsync(long orderId, long userId);
        Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(long orderId, long userId);
    }
}
