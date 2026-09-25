namespace Elara.Application.DTOs
{
    public class ApplyReferralResultDto
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int RewardPoints { get; set; }
    }
    
}