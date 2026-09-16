using Elara.Domain.Common;

namespace Elara.Domain.Entities.Auth
{
    public class RefreshToken : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string CreatedByIp { get; set; } = null!;
        public string? ReplacedByToken { get; set; }

        public bool IsActive => !IsRevoked && Expires > DateTime.UtcNow;
    }
}
