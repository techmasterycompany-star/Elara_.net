using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Review : SoftDelete
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
    }
}
