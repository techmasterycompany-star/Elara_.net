namespace Elara.Application.DTOs.Auth
{
    public class PhoneLoginRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string Otp { get; set; } = null!;

    }
}
