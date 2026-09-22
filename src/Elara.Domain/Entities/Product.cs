using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Product : SoftDelete
    {
        public long SellerProfileId { get; set; }
        public SellerProfile SellerProfile { get; set; } = null!;

        public long CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ProductImage> Images { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<Wishlist> Wishlists { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
