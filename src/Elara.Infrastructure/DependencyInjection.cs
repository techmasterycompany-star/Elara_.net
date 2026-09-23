using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Repository.Auth;
using Elara.Application.Interfaces.Service;
using Elara.Application.Interfaces.Service.Auth;
using Elara.Application.Services;
using Elara.Infrastructure.Data;
using Elara.Infrastructure.Repositories;
using Elara.Infrastructure.Repositories.Auth;
using Elara.Infrastructure.Services;
using Elara.Infrastructure.Services.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<DbSeeder>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
            services.AddScoped<IEmailConfirmationRepository, EmailConfirmationRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<ICheckoutRepository, CheckoutRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IShippingMethodRepository, ShippingMethodRepository>();
            services.AddScoped<ISellerApplicationRepository, SellerApplicationRepository>();
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

            services.AddScoped<IStorageService, CloudinaryStorageService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IHomepageSectionRepository, HomepageSectionRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();

            // Payment Gateway Services
            services.AddHttpClient<IPayPalPaymentService, PayPalPaymentService>();
            services.AddScoped<IStripePaymentService, StripePaymentService>();
            services.AddScoped<IWalletService, WalletService>();

            return services;
        }
    }
}
