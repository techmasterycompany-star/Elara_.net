using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public long CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
    }
}
