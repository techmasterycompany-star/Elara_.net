using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class CustomerOrderStatusHistoryDto
    {
        public long Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
