using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class User : SoftDelete
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = []; 
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
        public SellerProfile? SellerProfile { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
        public ICollection<Transaction> TransactionTypes { get; set; } = [];
        public ICollection<SellerApplication> SellerApplications { get; set; } = [];
    }
}
