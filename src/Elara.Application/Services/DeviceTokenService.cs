using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IDeviceTokenRepository _repository;

        public DeviceTokenService(IDeviceTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<RegisterDeviceResultDto> RegisterAsync(
            long userId,
            string token,
            DevicePlatform platform,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByTokenAsync(token, cancellationToken);

            if (existing != null)
                return new RegisterDeviceResultDto { IsSuccess = true };

            var deviceToken = new DeviceToken
            {
                UserId = userId,
                Token = token,
                Platform = platform,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(deviceToken, cancellationToken);

            return new RegisterDeviceResultDto { IsSuccess = true };
        }

        public async Task<RegisterDeviceResultDto> UnregisterAsync(
            string token,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByTokenAsync(token, cancellationToken);

            if (existing == null)
                return Invalid("DEVICE_NOT_FOUND", "This device token is not registered.");

            await _repository.DeactivateAsync(token, cancellationToken);

            return new RegisterDeviceResultDto { IsSuccess = true };
        }

        private static RegisterDeviceResultDto Invalid(string errorCode, string message) => new()
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            ErrorMessage = message
        };
    }
}