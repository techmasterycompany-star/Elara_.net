using Elara.Application.Interfaces.Repository.Auth;
using Elara.Domain.Entities.Auth;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories.Auth
{
    public class EmailConfirmationRepository : IEmailConfirmationRepository
    {
        private readonly AppDbContext context;
        public EmailConfirmationRepository(AppDbContext context) => this.context = context;
       

        public async Task AddAsync(EmailConfirmation confirmation)
        {
            await context.EmailConfirmations.AddAsync(confirmation);
            await context.SaveChangesAsync();
        }

        public async Task<EmailConfirmation?> GetValidTokenAsync(string email, string token) =>
             await context.EmailConfirmations
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Email == email
                    && c.Token == token
                    && !c.IsUsed
                    && c.ExpiresAt > DateTime.UtcNow);
        

        public async Task MarkAsUsedAsync(EmailConfirmation confirmation)
        {
            confirmation.IsUsed = true;
            await context.SaveChangesAsync();
        }
        public async Task InvalidateAllAsync(long userId)
        {
            var tokens = await context.EmailConfirmations
                .Where(c => c.UserId == userId && !c.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
                token.IsUsed = true;

            await context.SaveChangesAsync();
        }
    }
}
