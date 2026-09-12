using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public NotificationType Type { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
