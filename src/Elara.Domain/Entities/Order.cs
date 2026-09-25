using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Order : SoftDelete
    {
        public long? UserId { get; set; }
        public User? User { get; set; }
        public string? GuestSessionId { get; set; }

        public string? GuestFullName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhoneNumber { get; set; }

        public string ShippingFullName { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingState { get; set; } = null!;
        public string ShippingPostalCode { get; set; } = null!;
        public string ShippingCountry { get; set; } = null!;

        public long ShippingMethodId { get; set; }
        public ShippingMethod ShippingMethod { get; set; } = null!;

        public long? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }

        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
        public Payment? Payment { get; set; }
        public ICollection<Shipment> Shipments { get; set; } = [];
        public ICollection<Transaction> LoyaltyTransactions { get; set; } = [];
    }
}
