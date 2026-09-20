using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface INewsletterSubscriptionRepository
    {
        Task<NewsletterSubscription?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(NewsletterSubscription subscription, CancellationToken cancellationToken = default);
        Task UpdateAsync(NewsletterSubscription subscription, CancellationToken cancellationToken = default);
        Task<List<NewsletterSubscription>> GetAllSubscribedAsync(CancellationToken cancellationToken = default);
    }
}