using BCrypt.Net;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using DomainRole = Elara.Domain.Entities.Role;

namespace Elara.Infrastructure.Data
{
    public class DbSeeder
    {
        private const string DefaultPassword = "Admin@123!";

        private readonly AppDbContext _context;

        public DbSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if(await _context.Roles.AnyAsync(cancellationToken))
            {
                // Database has already been seeded
                return;
            }
            await SeedRolesAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
            await SeedUserRolesAsync(cancellationToken);
            await SeedSellerProfilesAsync(cancellationToken);
            await SeedCategoriesAsync(cancellationToken);
            await SeedShippingMethodsAsync(cancellationToken);
            await SeedPromoCodesAsync(cancellationToken);
        }

        private async Task SeedRolesAsync(CancellationToken cancellationToken)
        {
            await EnsureRoleAsync("Admin", cancellationToken);
            await EnsureRoleAsync("Seller", cancellationToken);
            await EnsureRoleAsync("Customer", cancellationToken);
        }

        private async Task EnsureRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var exists = await _context.Roles.AnyAsync(x => x.Name == roleName, cancellationToken);
            if (!exists)
            {
                await _context.Roles.AddAsync(new DomainRole
                {
                    Name = roleName
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedUsersAsync(CancellationToken cancellationToken)
        {
            await EnsureUserAsync(
                username: "admin",
                email: "admin@elara.com",
                phoneNumber: "5551000001",
                fullName: "System Administrator",
                cancellationToken);

            await EnsureUserAsync(
                username: "seller",
                email: "seller@elara.com",
                phoneNumber: "5551000002",
                fullName: "Demo Seller",
                cancellationToken);

            await EnsureUserAsync(
                username: "customer",
                email: "customer@elara.com",
                phoneNumber: "5551000003",
                fullName: "Demo Customer",
                cancellationToken);
        }

        private async Task EnsureUserAsync(
            string username,
            string email,
            string phoneNumber,
            string fullName,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Users.AnyAsync(x => x.Username == username || x.Email == email, cancellationToken);
            if (!exists)
            {
                await _context.Users.AddAsync(new User
                {
                    Username = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsActive = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword)
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedUserRolesAsync(CancellationToken cancellationToken)
        {
            var admin = await _context.Users.FirstAsync(x => x.Username == "admin", cancellationToken);
            var seller = await _context.Users.FirstAsync(x => x.Username == "seller", cancellationToken);
            var customer = await _context.Users.FirstAsync(x => x.Username == "customer", cancellationToken);

            var adminRole = await _context.Roles.FirstAsync(x => x.Name == "Admin", cancellationToken);
            var sellerRole = await _context.Roles.FirstAsync(x => x.Name == "Seller", cancellationToken);
            var customerRole = await _context.Roles.FirstAsync(x => x.Name == "Customer", cancellationToken);

            await EnsureUserRoleAsync(admin.Id, adminRole.Id, cancellationToken);
            await EnsureUserRoleAsync(seller.Id, sellerRole.Id, cancellationToken);
            await EnsureUserRoleAsync(customer.Id, customerRole.Id, cancellationToken);
        }

        private async Task EnsureUserRoleAsync(long userId, long roleId, CancellationToken cancellationToken)
        {
            var exists = await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);
            if (!exists)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedSellerProfilesAsync(CancellationToken cancellationToken)
        {
            var seller = await _context.Users.FirstAsync(x => x.Username == "seller", cancellationToken);

            var exists = await _context.SellerProfiles.AnyAsync(x => x.UserId == seller.Id, cancellationToken);
            if (!exists)
            {
                await _context.SellerProfiles.AddAsync(new SellerProfile
                {
                    UserId = seller.Id,
                    StoreName = "Elara Store",
                    StoreDescription = "Demo seller profile for initial development data.",
                    IsApproved = true
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
        {
            await EnsureCategoryAsync("Electronics", "Devices, gadgets, and accessories.", cancellationToken);
            await EnsureCategoryAsync("Fashion", "Clothing, footwear, and accessories.", cancellationToken);
            await EnsureCategoryAsync("Home & Kitchen", "Essentials for home living and cooking.", cancellationToken);
        }

        private async Task EnsureCategoryAsync(string name, string description, CancellationToken cancellationToken)
        {
            var exists = await _context.Categories.AnyAsync(x => x.Name == name, cancellationToken);
            if (!exists)
            {
                await _context.Categories.AddAsync(new Category
                {
                    Name = name,
                    Description = description,
                    IsDeleted = false
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedShippingMethodsAsync(CancellationToken cancellationToken)
        {
            await EnsureShippingMethodAsync(
                name: "Standard Shipping",
                description: "Economical delivery option for non-urgent orders.",
                baseCost: 5.99m,
                estimatedDays: 5,
                cancellationToken);

            await EnsureShippingMethodAsync(
                name: "Express Shipping",
                description: "Faster delivery for customers who need their order sooner.",
                baseCost: 12.99m,
                estimatedDays: 2,
                cancellationToken);
        }

        private async Task EnsureShippingMethodAsync(
            string name,
            string description,
            decimal baseCost,
            int estimatedDays,
            CancellationToken cancellationToken)
        {
            var exists = await _context.ShippingMethods.AnyAsync(x => x.Name == name, cancellationToken);
            if (!exists)
            {
                await _context.ShippingMethods.AddAsync(new ShippingMethod
                {
                    Name = name,
                    Description = description,
                    BaseCost = baseCost,
                    EstimatedDays = estimatedDays,
                    IsActive = true,
                    IsDeleted = false
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedPromoCodesAsync(CancellationToken cancellationToken)
        {
            var exists = await _context.PromoCodes.AnyAsync(x => x.Code == "WELCOME10", cancellationToken);
            if (!exists)
            {
                await _context.PromoCodes.AddAsync(new PromoCode
                {
                    Code = "WELCOME10",
                    DiscountType = DiscountType.Percentage,
                    DiscountValue = 10m,
                    MinOrderAmount = 50m,
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    UsageLimit = 1000,
                    TimesUsed = 0,
                    IsActive = true,
                    IsDeleted = false
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
