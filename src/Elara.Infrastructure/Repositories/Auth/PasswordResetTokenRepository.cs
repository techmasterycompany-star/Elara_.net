using Elara.Application.Interfaces.Repository.Auth;
using Elara.Domain.Entities.Auth;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories.Auth
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDbContext context;
        public PasswordResetTokenRepository(AppDbContext context) => this.context = context;
        

        public async Task AddAsync(PasswordResetToken passwordResetToken)
        {
            await context.Set<PasswordResetToken>().AddAsync(passwordResetToken);
            await context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string email, string token) =>
             await context.Set<PasswordResetToken>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Email == email
                    && c.Token == token
                    && !c.IsUsed
                    && c.ExpiresAt > DateTime.UtcNow);
        

        public async Task MarkAsUsedAsync(PasswordResetToken passwordResetToken)
        {
            passwordResetToken.IsUsed = true;
            await context.SaveChangesAsync();
        }
    }
}
