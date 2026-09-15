using Elara.Domain.Common;

namespace Elara.Domain.Entities.Auth
{
    public class EmailConfirmation : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
