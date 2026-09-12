using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class PaymentMethod : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string Provider { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Last4Digits { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsDefault { get; set; }
    }
}
