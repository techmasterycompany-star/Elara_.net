using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public long OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public PaymentMethodType Method { get; set; }
        public string Provider { get; set; } = null!;
        public string TransactionId { get; set; } = null!;

        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
