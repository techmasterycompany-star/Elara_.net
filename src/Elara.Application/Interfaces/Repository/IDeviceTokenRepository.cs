using Elara.Domain.Entities;

namespace Elara.Application.Interfaces
{
    public interface IDeviceTokenRepository
    {
        Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<List<DeviceToken>> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);
        Task AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
        Task DeactivateAsync(string token, CancellationToken cancellationToken = default);
    }
}