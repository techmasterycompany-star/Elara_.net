using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class NewsletterSubscriptionRepository : INewsletterSubscriptionRepository
    {
        private readonly AppDbContext _context;

        public NewsletterSubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NewsletterSubscription?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
        }

        public async Task AddAsync(NewsletterSubscription subscription, CancellationToken cancellationToken = default)
        {
            await _context.NewsletterSubscriptions.AddAsync(subscription, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(NewsletterSubscription subscription, CancellationToken cancellationToken = default)
        {
            _context.NewsletterSubscriptions.Update(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<NewsletterSubscription>> GetAllSubscribedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.NewsletterSubscriptions
                .Where(s => s.IsSubscribed)
                .ToListAsync(cancellationToken);
        }
    }
}