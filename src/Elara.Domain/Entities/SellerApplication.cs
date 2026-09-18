using Elara.Domain.Common;
using Elara.Domain.Enums;


namespace Elara.Domain.Entities
{
    public class SellerApplication : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;

        public SellerApplicationStatus Status { get; set; } = SellerApplicationStatus.Pending;

        public string? RejectionReason { get; set; }
    }
}
