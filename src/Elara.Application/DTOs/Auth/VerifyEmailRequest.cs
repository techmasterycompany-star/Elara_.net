namespace Elara.Application.DTOs.Auth
{
    public class VerifyEmailRequest
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
