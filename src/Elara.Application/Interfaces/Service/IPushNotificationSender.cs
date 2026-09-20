namespace Elara.Application.Interfaces
{
    public interface IPushNotificationSender
    {
        Task SendAsync(
            List<string> deviceTokens,
            string title,
            string body,
            CancellationToken cancellationToken = default);
    }
}