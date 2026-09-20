namespace Elara.Application.Interfaces.Service
{
    public interface IEmailCampaignSender
    {
        Task SendAsync(List<string> recipientEmails, string subject, string body, CancellationToken cancellationToken = default);
    }
}