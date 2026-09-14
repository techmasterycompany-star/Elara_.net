namespace Elara.Application.DTOs.User
{
    public class UserDetailsDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<string> Roles { get; set; } = [];
    }


}
