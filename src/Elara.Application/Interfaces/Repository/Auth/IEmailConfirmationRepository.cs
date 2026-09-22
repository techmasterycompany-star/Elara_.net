using Elara.Domain.Entities.Auth;

namespace Elara.Application.Interfaces.Repository.Auth
{
    public interface IEmailConfirmationRepository
    {
        Task AddAsync(EmailConfirmation confirmation);
        Task<EmailConfirmation?> GetValidTokenAsync(string email, string token);
        Task MarkAsUsedAsync(EmailConfirmation confirmation);
        Task InvalidateAllAsync(long userId);
    }
}
