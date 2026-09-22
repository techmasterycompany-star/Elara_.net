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

    public class CustomerOrderListDto
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }


    public class CustomerOrderDetailsDto
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public string ShippingFullName { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingState { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
        public string ShippingCountry { get; set; } = null!;

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<CustomerOrderItemDto> Items { get; set; } = [];
    }


    public class CustomerOrderItemDto
    {
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }


    public class CustomerOrderStatusHistoryDto
    {
        public long Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
