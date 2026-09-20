using Elara.Application.Interfaces.Service;

namespace Elara.Infrastructure.Services
{
    public class SmtpEmailCampaignSender : IEmailCampaignSender
    {
        public Task SendAsync(List<string> recipientEmails, string subject, string body, CancellationToken cancellationToken = default)
        {
            // TODO: wire up actual provider (SendGrid, SMTP, etc.)
            // Placeholder so the flow compiles and runs end-to-end for now.
            return Task.CompletedTask;
        }
    }
}