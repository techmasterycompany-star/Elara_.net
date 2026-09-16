using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class OrderStatusHistoryDto
    {
        public long Id { get; set; }

        public OrderStatus Status { get; set; }
        public string Notes { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}
