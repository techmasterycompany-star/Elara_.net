using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Referral
    {
        public int Id { get; set; }
        public long ReferrerUserId { get; set; }
        public long ReferredUserId { get; set; }
        public string ReferralCode { get; set; }
        public ReferralStatus Status { get; set; }
        public int RewardPoints { get; set; }
        public DateTime CreatedAt { get; set; }

        public User ReferrerUser { get; set; }
        public User ReferredUser { get; set; }
    }
}
