using Elara.Application.DTOs.Order;
using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IOrderRepository 
    { 
        Task<PaginationQueryResult<Order>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest);
        Task<Order?> GetOrderByIdAsync(long id);
        Task<Order?> GetOrderDetailsAsync(long id);
        Task UpdateOrderAsync(Order order);
        Task<bool> HasUserPurchasedProductAsync(long userId, long productId);
    }
}
