namespace Elara.Application.DTOs
{
    public class SendCampaignResultDto
    {
        public bool IsSuccess { get; set; }
        public int RecipientsCount { get; set; }
        public string? ErrorMessage { get; set; }
    }
}