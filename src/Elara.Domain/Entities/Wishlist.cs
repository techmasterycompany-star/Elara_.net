using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public DateTime AddedAt { get; set; }
    }
}
