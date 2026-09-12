using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public long? OrderId { get; set; }
        public Order? Order { get; set; }

        public int Points { get; set; }
        public TransactionType Type { get; set; }
    }
}
