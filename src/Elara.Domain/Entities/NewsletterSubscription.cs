namespace Elara.Domain.Entities
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }
        public long? UserId { get; set; }          // nullable - guests can subscribe too
        public string Email { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime SubscribedAt { get; set; }
        public DateTime? UnsubscribedAt { get; set; }

        public User User { get; set; }
    }
}
