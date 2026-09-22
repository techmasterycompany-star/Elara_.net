using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class CustomerOrderListDto
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
