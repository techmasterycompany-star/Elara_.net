using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class WalletTransaction : SoftDelete
    {
        public long WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;

        public WalletTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = null!;

        public long? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}