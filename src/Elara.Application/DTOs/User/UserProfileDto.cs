namespace Elara.Application.DTOs.User
{
    public class UserProfileDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = [];
        public List<AddressDto> Addresses { get; set; } = [];
    }
}
