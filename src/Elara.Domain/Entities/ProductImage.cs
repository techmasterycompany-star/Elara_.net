using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
        public string ImagePublicId { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
