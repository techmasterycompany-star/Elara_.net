using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Elara.Application.DTOs.Order
{
    public class UpdateOrderStatusRequestDto
    {
        [Required]
        public OrderStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class GetMyOrdersRequest : PaginationRequest<OrderSortBy>
    {
    }
}
