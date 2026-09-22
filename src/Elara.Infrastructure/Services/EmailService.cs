using Elara.Application.Interfaces.Service.Auth;
using Elara.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Elara.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
        {
            this.emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string email, string token)
        {
            var clientUrl = emailOptions.ClientUrl ?? "https://localhost:7067";
            var link = $"{clientUrl}/verify-email.html?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";

            var body = $@"
                <div style='font-family:Arial;text-align:center;padding:30px;'>
                    <h2 style='color:#C2185B;'>Welcome to Elara!</h2>
                    <p>Please verify your email by clicking the button below:</p>
                    <a href='{link}' style='background-color:#C2185B;color:white;padding:14px 28px;text-decoration:none;border-radius:6px;display:inline-block;margin-top:10px;'>
                        Verify Email
                    </a>
                    <p style='margin-top:20px;font-size:13px;'>If the button doesn't work, copy this link:<br>{link}</p>
                </div>";

            await SendAsync(email, "Verify your Elara email", body);
        }

        public async Task SendPasswordResetAsync(string email, string token)
        {
            var clientUrl = emailOptions.ClientUrl ?? "https://localhost:7067";
            var link = $"{clientUrl}/reset-password.html?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";

            var body = $@"
                <div style='font-family:Arial;text-align:center;padding:30px;'>
                    <h2 style='color:#1565C0;'>Reset your password</h2>
                    <p>Click the button below to reset your password:</p>
                    <a href='{link}' style='background-color:#1565C0;color:white;padding:14px 28px;text-decoration:none;border-radius:6px;display:inline-block;margin-top:10px;'>
                        Reset Password
                    </a>
                    <p style='margin-top:20px;font-size:13px;'>If the button doesn't work, copy this link:<br>{link}</p>
                </div>";

            await SendAsync(email, "Reset your Elara password", body);
        }

        private async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = emailOptions.Smtp.Host ?? "smtp.gmail.com";
            var port = emailOptions.Smtp.Port > 0 ? emailOptions.Smtp.Port : 587;
            var username = emailOptions.Smtp.Username!;
            var password = emailOptions.Smtp.Password!;
            var from = emailOptions.Smtp.From ?? username;
            using var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            try
            {
                await smtp.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw new InvalidOperationException("Failed to send email. Check SMTP settings.");
            }
        }
    }
}
