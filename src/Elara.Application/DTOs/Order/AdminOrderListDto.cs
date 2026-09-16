using Elara.Application.DTOs.Common;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Elara.Application.DTOs.Order
{
    public class AdminOrderListDto
    {
        public long Id { get; set; }

        public long? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }

        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public int ShipmentsCount { get; set; }
    }

}
