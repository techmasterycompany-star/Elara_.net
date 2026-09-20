using Elara.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Elara.Infrastructure.Services
{
    public class FcmPushNotificationSender : IPushNotificationSender
    {
        private readonly ILogger<FcmPushNotificationSender> _logger;

        public FcmPushNotificationSender(ILogger<FcmPushNotificationSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(
            List<string> deviceTokens,
            string title,
            string body,
            CancellationToken cancellationToken = default)
        {
            // TODO: wire up Firebase Admin SDK (FCM) or OneSignal here.
            _logger.LogInformation(
                "Push notification queued for {Count} devices: {Title}",
                deviceTokens.Count,
                title);

            return Task.CompletedTask;
        }
    }
}