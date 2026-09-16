using Elara.Domain.Enums;

namespace Elara.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Role Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
    }
}
