using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }

        public ICollection<ShipmentItem> ShipmentItems { get; set; } = [];
    }
}
