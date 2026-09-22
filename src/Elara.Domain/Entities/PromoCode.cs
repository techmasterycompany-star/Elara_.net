using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class PromoCode : SoftDelete
    {
        public string Code { get; set; } = null!;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Order> Orders { get; set; } = [];
    }
}
