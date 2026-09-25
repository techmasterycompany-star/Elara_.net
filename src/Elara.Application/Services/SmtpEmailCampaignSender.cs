using Elara.Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Elara.Infrastructure.Services
{
    public class SmtpEmailCampaignSender : IEmailCampaignSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailCampaignSender> _logger;

        public SmtpEmailCampaignSender(
            IConfiguration configuration,
            ILogger<SmtpEmailCampaignSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(
            List<string> recipientEmails,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            var host = _configuration["Smtp:Host"];
            var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var fromName = _configuration["Smtp:FromName"] ?? "Elara";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(fromEmail!);

            const int batchSize = 50;
            var batches = recipientEmails
                .Select((email, index) => new { email, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.email).ToList());

            foreach (var batch in batches)
            {
                message.Bcc.Clear();
                foreach (var email in batch)
                {
                    message.Bcc.Add(email);
                }

                try
                {
                    await client.SendMailAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send newsletter batch to {Count} recipients", batch.Count);
                    throw;
                }
            }
        }
    }
}