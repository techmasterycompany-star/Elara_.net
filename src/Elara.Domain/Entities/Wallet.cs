using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Wallet : SoftDelete
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal Balance { get; set; }
        public string Currency { get; set; } = "USD";

        public ICollection<WalletTransaction> Transactions { get; set; } = [];
    }
}