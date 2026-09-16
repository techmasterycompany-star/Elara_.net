using Elara.Domain.Entities.Auth;

namespace Elara.Application.Interfaces.Repository.Auth
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken passwordResetToken);
        Task<PasswordResetToken?> GetValidTokenAsync(string email, string token);
        Task MarkAsUsedAsync(PasswordResetToken passwordResetToken);
    }
}
