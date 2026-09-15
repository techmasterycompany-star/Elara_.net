using Elara.Application.Interfaces.Repository.Auth;
using Elara.Domain.Entities.Auth;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories.Auth
{
    public class RevokedTokenRepository : IRevokedTokenRepository
    {
        private readonly AppDbContext context;
        public RevokedTokenRepository(AppDbContext context) => this.context = context;
       

        public async Task AddAsync(RevokedToken revokedToken)
        {
            await context.Set<RevokedToken>().AddAsync(revokedToken);
            await context.SaveChangesAsync();
        }
        public async Task<bool> IsRevokedAsync(string jti) =>
            await context.Set<RevokedToken>()
                .AnyAsync(rt => rt.Jti == jti);

    }
}
