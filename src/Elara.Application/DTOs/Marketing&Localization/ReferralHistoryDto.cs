namespace Elara.Application.DTOs
{
    public class ReferralHistoryDto
    {
        public long ReferredUserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RewardPoints { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}