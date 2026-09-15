using Elara.Domain.Common;

namespace Elara.Domain.Entities.Auth
{
    public class RevokedToken : BaseEntity
    {
        public string Jti { get; set; } = null!;
        public DateTime RevokedAt { get; set; }
        public string? Reason { get; set; }
    }
}
