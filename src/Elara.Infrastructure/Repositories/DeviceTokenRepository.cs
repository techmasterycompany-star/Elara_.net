using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Repositories
{
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly AppDbContext _context;

        public DeviceTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .FirstOrDefaultAsync(d => d.Token == token, cancellationToken);
        }

        public async Task<List<DeviceToken>> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens
                .Where(d => d.UserId == userId && d.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
        {
            await _context.DeviceTokens.AddAsync(deviceToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAsync(string token, CancellationToken cancellationToken = default)
        {
            var deviceToken = await GetByTokenAsync(token, cancellationToken);
            if (deviceToken != null)
            {
                deviceToken.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}