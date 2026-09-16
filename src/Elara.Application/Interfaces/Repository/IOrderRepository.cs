using Elara.Application.DTOs.Order;
using Elara.Domain.Entities;

namespace Elara.Application.Interfaces.Repository
{
    public interface IOrderRepository 
    { 
        Task<IEnumerable<Order>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest);
        Task<Order?> GetOrderByIdAsync(long id);
        Task<Order?> GetOrderDetailsAsync(long id);
        Task UpdateOrderAsync(Order order);
    }
}
