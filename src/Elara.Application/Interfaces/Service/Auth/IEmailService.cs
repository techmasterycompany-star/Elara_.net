namespace Elara.Application.Interfaces.Service.Auth
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string token);
        Task SendPasswordResetAsync(string email, string token);
    }
}
