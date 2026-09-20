namespace Elara.Application.DTOs
{
    public class NewsletterSubscriberDto
    {
        public string Email { get; set; } = string.Empty;
        public bool IsSubscribed { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}