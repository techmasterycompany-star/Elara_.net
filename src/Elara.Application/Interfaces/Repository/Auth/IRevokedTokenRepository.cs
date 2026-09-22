using Elara.Domain.Entities.Auth;

namespace Elara.Application.Interfaces.Repository.Auth
{
    public interface IRevokedTokenRepository
    {
        Task AddAsync(RevokedToken revokedToken);
        Task<bool> IsRevokedAsync(string jti);
    }
}
