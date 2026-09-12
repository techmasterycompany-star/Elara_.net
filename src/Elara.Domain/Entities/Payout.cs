using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Payout : BaseEntity
    {
        public long SellerProfileId { get; set; }
        public SellerProfile SellerProfile { get; set; } = null!;

        public decimal Amount { get; set; }
        public PayoutStatus Status { get; set; }
        public DateTime? PayoutDate { get; set; }
    }
}
