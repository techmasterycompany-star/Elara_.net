using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public long? UserId { get; set; }
        public User? User { get; set; }

        public string? GuestSessionId { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
