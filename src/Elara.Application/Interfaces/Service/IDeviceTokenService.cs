using Elara.Application.DTOs;
using Elara.Domain.Enums;

namespace Elara.Application.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<RegisterDeviceResultDto> RegisterAsync(
            long userId,
            string token,
            DevicePlatform platform,
            CancellationToken cancellationToken = default);

        Task<RegisterDeviceResultDto> UnregisterAsync(
            string token,
            CancellationToken cancellationToken = default);
    }
}