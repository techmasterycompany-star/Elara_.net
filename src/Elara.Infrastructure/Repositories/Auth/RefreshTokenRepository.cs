using Elara.Application.Interfaces.Repository.Auth;
using Elara.Domain.Entities.Auth;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext context;
        public RefreshTokenRepository(AppDbContext context) => this.context = context;
       

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
             await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        

        public async Task<RefreshToken?> GetActiveTokenByUserIdAsync(long userId) =>
             await context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);
        

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            context.RefreshTokens.Update(refreshToken);
            await context.SaveChangesAsync();
        }
    }
}
