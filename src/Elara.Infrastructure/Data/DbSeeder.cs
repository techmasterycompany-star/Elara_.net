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
            await SeedRolesAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
            await SeedUserRolesAsync(cancellationToken);
            await SeedSellerProfilesAsync(cancellationToken);
            await SeedCategoriesAsync(cancellationToken);
            await SeedShippingMethodsAsync(cancellationToken);
            await SeedPromoCodesAsync(cancellationToken);
            await SeedProductsAsync(cancellationToken);
            await SeedOrdersAsync(cancellationToken);
        }

        private async Task SeedRolesAsync(CancellationToken cancellationToken)
        {
            await EnsureRoleAsync("Admin", cancellationToken);
            await EnsureRoleAsync("Seller", cancellationToken);
            await EnsureRoleAsync("Customer", cancellationToken);
        }

        private async Task EnsureRoleAsync(
            string roleName,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Roles
                .AnyAsync(x => x.Name == roleName, cancellationToken);

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
                username: "seller2",
                email: "seller2@elara.com",
                phoneNumber: "5551000004",
                fullName: "Demo Seller 2",
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
            var exists = await _context.Users
                .AnyAsync(
                    x => x.Username == username || x.Email == email,
                    cancellationToken);

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
            var admin = await _context.Users
                .FirstAsync(x => x.Username == "admin", cancellationToken);

            var seller = await _context.Users
                .FirstAsync(x => x.Username == "seller", cancellationToken);

            var seller2 = await _context.Users
                .FirstAsync(x => x.Username == "seller2", cancellationToken);

            var customer = await _context.Users
                .FirstAsync(x => x.Username == "customer", cancellationToken);

            var adminRole = await _context.Roles
                .FirstAsync(x => x.Name == "Admin", cancellationToken);

            var sellerRole = await _context.Roles
                .FirstAsync(x => x.Name == "Seller", cancellationToken);

            var customerRole = await _context.Roles
                .FirstAsync(x => x.Name == "Customer", cancellationToken);

            await EnsureUserRoleAsync(
                admin.Id,
                adminRole.Id,
                cancellationToken);

            await EnsureUserRoleAsync(
                seller.Id,
                sellerRole.Id,
                cancellationToken);

            await EnsureUserRoleAsync(
                seller2.Id,
                sellerRole.Id,
                cancellationToken);

            await EnsureUserRoleAsync(
                customer.Id,
                customerRole.Id,
                cancellationToken);
        }

        private async Task EnsureUserRoleAsync(
            long userId,
            long roleId,
            CancellationToken cancellationToken)
        {
            var exists = await _context.UserRoles
                .AnyAsync(
                    x => x.UserId == userId && x.RoleId == roleId,
                    cancellationToken);

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

        private async Task SeedSellerProfilesAsync(
            CancellationToken cancellationToken)
        {
            var seller = await _context.Users
                .FirstAsync(x => x.Username == "seller", cancellationToken);

            var seller2 = await _context.Users
                .FirstAsync(x => x.Username == "seller2", cancellationToken);

            await EnsureSellerProfileAsync(
                seller.Id,
                "Elara Store",
                "Demo seller profile for initial development data.",
                cancellationToken);

            await EnsureSellerProfileAsync(
                seller2.Id,
                "Elara Fashion",
                "Second demo seller for marketplace testing.",
                cancellationToken);
        }

        private async Task EnsureSellerProfileAsync(
            long userId,
            string storeName,
            string description,
            CancellationToken cancellationToken)
        {
            var exists = await _context.SellerProfiles
                .AnyAsync(x => x.UserId == userId, cancellationToken);

            if (!exists)
            {
                await _context.SellerProfiles.AddAsync(new SellerProfile
                {
                    UserId = userId,
                    StoreName = storeName,
                    StoreDescription = description,
                    IsApproved = true
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task SeedCategoriesAsync(
            CancellationToken cancellationToken)
        {
            await EnsureCategoryAsync(
                "Electronics",
                "Devices, gadgets, and accessories.",
                cancellationToken);

            await EnsureCategoryAsync(
                "Fashion",
                "Clothing, footwear, and accessories.",
                cancellationToken);

            await EnsureCategoryAsync(
                "Home & Kitchen",
                "Essentials for home living and cooking.",
                cancellationToken);
        }

        private async Task EnsureCategoryAsync(
            string name,
            string description,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Categories
                .AnyAsync(x => x.Name == name, cancellationToken);

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

        private async Task SeedShippingMethodsAsync(
            CancellationToken cancellationToken)
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
            var exists = await _context.ShippingMethods
                .AnyAsync(x => x.Name == name, cancellationToken);

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

        private async Task SeedPromoCodesAsync(
            CancellationToken cancellationToken)
        {
            var exists = await _context.PromoCodes
                .AnyAsync(
                    x => x.Code == "WELCOME10",
                    cancellationToken);

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

        private async Task SeedProductsAsync(CancellationToken cancellationToken)
        {
            var seller = await _context.Users
                .FirstAsync(x => x.Username == "seller", cancellationToken);

            var seller2 = await _context.Users
                .FirstAsync(x => x.Username == "seller2", cancellationToken);

            var sellerProfile = await _context.SellerProfiles
                .FirstAsync(x => x.UserId == seller.Id, cancellationToken);

            var sellerProfile2 = await _context.SellerProfiles
                .FirstAsync(x => x.UserId == seller2.Id, cancellationToken);

            var electronics = await _context.Categories
                .FirstAsync(x => x.Name == "Electronics", cancellationToken);

            var fashion = await _context.Categories
                .FirstAsync(x => x.Name == "Fashion", cancellationToken);

            await EnsureProductAsync(
                "Wireless Headphones",
                "Demo wireless headphones",
                100m,
                50,
                sellerProfile.Id,
                electronics.Id,
                cancellationToken);

            await EnsureProductAsync(
                "Mechanical Keyboard",
                "Demo mechanical keyboard",
                150m,
                30,
                sellerProfile.Id,
                electronics.Id,
                cancellationToken);

            await EnsureProductAsync(
                "Classic T-Shirt",
                "Demo classic t-shirt",
                40m,
                100,
                sellerProfile2.Id,
                fashion.Id,
                cancellationToken);
        }

        private async Task EnsureProductAsync(
    string name,
    string description,
    decimal price,
    int stockQuantity,
    long sellerProfileId,
    long categoryId,
    CancellationToken cancellationToken)
        {
            var exists = await _context.Products.AnyAsync(
                x => x.Name == name &&
                     x.SellerProfileId == sellerProfileId,
                cancellationToken);

            if (exists)
                return;

            await _context.Products.AddAsync(new Product
            {
                Name = name,
                Description = description,
                Price = price,
                StockQuantity = stockQuantity,
                SellerProfileId = sellerProfileId,
                CategoryId = categoryId,
                IsActive = true,
                IsDeleted = false
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedOrdersAsync(
            CancellationToken cancellationToken)
        {
            var customer = await _context.Users
                .FirstAsync(
                    x => x.Username == "customer",
                    cancellationToken);

            var seller = await _context.Users
                .FirstAsync(
                    x => x.Username == "seller",
                    cancellationToken);

            var seller2 = await _context.Users
                .FirstAsync(
                    x => x.Username == "seller2",
                    cancellationToken);

            var sellerProfile = await _context.SellerProfiles
                .FirstAsync(
                    x => x.UserId == seller.Id,
                    cancellationToken);

            var sellerProfile2 = await _context.SellerProfiles
                .FirstAsync(
                    x => x.UserId == seller2.Id,
                    cancellationToken);

            var shippingMethod = await _context.ShippingMethods
                .FirstAsync(
                    x => x.Name == "Standard Shipping",
                    cancellationToken);

            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(
                    x => x.Code == "WELCOME10",
                    cancellationToken);

            var sellerProduct = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.SellerProfileId == sellerProfile.Id,
                    cancellationToken);

            var sellerProduct2 = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.SellerProfileId == sellerProfile2.Id,
                    cancellationToken);

            if (sellerProduct == null || sellerProduct2 == null)
            {
                Console.WriteLine(
                    "Order seed skipped: products for both sellers are required.");

                return;
            }

            var existingOrder = await _context.Orders
                .AnyAsync(
                    x => x.UserId == customer.Id,
                    cancellationToken);

            if (existingOrder)
                return;

            var now = DateTime.UtcNow;
            var orderDate = now.AddDays(-3);

            var item1 = new OrderItem
            {
                ProductId = sellerProduct.Id,
                Quantity = 2,
                UnitPrice = sellerProduct.Price,
                Discount = 0,
                Subtotal = sellerProduct.Price * 2,
                CreatedAt = orderDate,
                UpdatedAt = orderDate
            };

            var item2 = new OrderItem
            {
                ProductId = sellerProduct.Id,
                Quantity = 1,
                UnitPrice = sellerProduct.Price,
                Discount = 0,
                Subtotal = sellerProduct.Price,
                CreatedAt = orderDate,
                UpdatedAt = orderDate
            };

            var item3 = new OrderItem
            {
                ProductId = sellerProduct2.Id,
                Quantity = 1,
                UnitPrice = sellerProduct2.Price,
                Discount = 0,
                Subtotal = sellerProduct2.Price,
                CreatedAt = orderDate,
                UpdatedAt = orderDate
            };

            var subtotal =
                item1.Subtotal +
                item2.Subtotal +
                item3.Subtotal;

            var order = new Order
            {
                UserId = customer.Id,

                ShippingFullName = customer.FullName,
                ShippingPhone = customer.PhoneNumber!,
                ShippingStreet = "123 Demo Street",
                ShippingCity = "Alexandria",
                ShippingState = "Alexandria",
                ShippingPostalCode = "21500",
                ShippingCountry = "Egypt",

                ShippingMethodId = shippingMethod.Id,
                PromoCodeId = promoCode?.Id,

                OrderDate = orderDate,
                Status = OrderStatus.Shipped,

                SubTotal = subtotal,
                DiscountAmount = 0,
                ShippingCost = shippingMethod.BaseCost,
                TotalAmount = subtotal + shippingMethod.BaseCost,

                CreatedAt = orderDate,
                UpdatedAt = now
            };

            item1.Order = order;
            item2.Order = order;
            item3.Order = order;

            order.Items.Add(item1);
            order.Items.Add(item2);
            order.Items.Add(item3);

            // Shipment 1 - Delivered
            var shipment1 = new Shipment
            {
                Order = order,
                SellerProfileId = sellerProfile.Id,

                Carrier = "DHL",
                TrackingNumber = "DHL-ELARA-001",
                Status = ShipmentStatus.Delivered,

                ShippedDate = now.AddDays(-2),
                EstimatedDeliveryDate = now.AddDays(-1),
                DeliveredDate = now.AddHours(-6),

                CreatedAt = orderDate,
                UpdatedAt = now.AddHours(-6)
            };

            // Shipment 2 - In Transit
            var shipment2 = new Shipment
            {
                Order = order,
                SellerProfileId = sellerProfile.Id,

                Carrier = "FedEx",
                TrackingNumber = "FDX-ELARA-002",
                Status = ShipmentStatus.InTransit,

                ShippedDate = now.AddDays(-1),
                EstimatedDeliveryDate = now.AddDays(2),

                CreatedAt = orderDate,
                UpdatedAt = now.AddDays(-1)
            };

            // Shipment 3 - Pending
            var shipment3 = new Shipment
            {
                Order = order,
                SellerProfileId = sellerProfile2.Id,

                Carrier = "Aramex",
                TrackingNumber = "ARM-ELARA-003",
                Status = ShipmentStatus.Pending,

                EstimatedDeliveryDate = now.AddDays(4),

                CreatedAt = orderDate,
                UpdatedAt = orderDate
            };

            order.Shipments.Add(shipment1);
            order.Shipments.Add(shipment2);
            order.Shipments.Add(shipment3);

            var shipmentItem1 = new ShipmentItem
            {
                Shipment = shipment1,
                OrderItem = item1,
                Quantity = item1.Quantity,
                CreatedAt = orderDate,
                UpdatedAt = shipment1.UpdatedAt
            };

            var shipmentItem2 = new ShipmentItem
            {
                Shipment = shipment2,
                OrderItem = item2,
                Quantity = item2.Quantity,
                CreatedAt = orderDate,
                UpdatedAt = shipment2.UpdatedAt
            };

            var shipmentItem3 = new ShipmentItem
            {
                Shipment = shipment3,
                OrderItem = item3,
                Quantity = item3.Quantity,
                CreatedAt = orderDate,
                UpdatedAt = shipment3.UpdatedAt
            };

            shipment1.Items.Add(shipmentItem1);
            shipment2.Items.Add(shipmentItem2);
            shipment3.Items.Add(shipmentItem3);

            await _context.Orders.AddAsync(
                order,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}