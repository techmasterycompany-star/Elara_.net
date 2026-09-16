using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;


namespace Elara.Application.DTOs.Order
{
    public class AdminOrderFilterDto : PaginationRequest<OrderSortBy>
    {
        public string? Search { get; set; }

        public OrderStatus? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public long? SellerId { get; set; }
        public long? CustomerId { get; set; }

    }

    public enum OrderSortBy
    {
        Id,
        OrderDate,
        TotalAmount,
        Status
    }
}
