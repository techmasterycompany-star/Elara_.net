using Elara.Domain.Common;
using Elara.Domain.Entities;
using Elara.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
        public DbSet<Payout> Payouts => Set<Payout>();
        public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
        public DbSet<EmailConfirmation> EmailConfirmations => Set<EmailConfirmation>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<SellerApplication> SellerApplications => Set<SellerApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            modelBuilder.Entity<User>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<SellerProfile>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Review>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Order>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<ShippingMethod>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<PromoCode>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Shipment>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = Elara.Domain.Enums.Role.Admin.ToString() },
                new Role { Id = 2, Name = Elara.Domain.Enums.Role.Seller.ToString() },
                new Role { Id = 3, Name = Elara.Domain.Enums.Role.Customer.ToString() }
            );

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
