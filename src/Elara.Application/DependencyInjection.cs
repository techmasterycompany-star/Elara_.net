using Elara.Application.Interfaces;
using Elara.Application.Interfaces.Service;
using Elara.Application.Interfaces.Service.Auth;
using Elara.Application.Services;
using Elara.Application.Services.Auth;
using Elara.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elara.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IDeviceTokenService, DeviceTokenService>();
            services.AddScoped<IPushNotificationSender, FcmPushNotificationSender>();
            services.AddScoped<INewsletterService, NewsletterService>();
            services.AddScoped<IEmailCampaignSender, SmtpEmailCampaignSender>();
            services.AddScoped<IReferralService, ReferralService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddScoped<ILoyaltyService, LoyaltyService>();



            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IShippingMethodService, ShippingMethodService>();
            services.AddScoped<ISellerApplicationService, SellerApplicationService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IHomepageService, HomepageService>();

            return services;
        }
    }
}
