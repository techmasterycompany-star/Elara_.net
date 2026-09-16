using System.Security.Claims;


namespace Elara.Application.Interfaces.Service.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(long userId, string email);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
