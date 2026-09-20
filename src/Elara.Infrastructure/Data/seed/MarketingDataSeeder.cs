using Elara.Domain.Entities;
using Elara.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Elara.Infrastructure.Data.Seed
{
    public static class MarketingDataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedLanguagesAsync(context);
            await SeedPromoCodesAsync(context);
            await SeedResourceStringsAsync(context);
            await SeedUserDependentDataAsync(context);
        }

        private static async Task SeedLanguagesAsync(AppDbContext context)
        {
            if (await context.Languages.AnyAsync())
                return;

            context.Languages.AddRange(
                new Language { Code = "en", Name = "English", IsActive = true, IsDefault = true },
                new Language { Code = "ar", Name = "Arabic", IsActive = true, IsDefault = false },
                new Language { Code = "fr", Name = "French", IsActive = false, IsDefault = false }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedPromoCodesAsync(AppDbContext context)
        {
            if (await context.PromoCodes.AnyAsync())
                return;

            context.PromoCodes.AddRange(
                new PromoCode
                {
                    Code = "WELCOME10",
                    DiscountType = DiscountType.Percentage,
                    DiscountValue = 10,
                    MinOrderAmount = 50,
                    ExpiryDate = DateTime.UtcNow.AddMonths(3),
                    UsageLimit = 100,
                    TimesUsed = 0,
                    IsActive = true
                },
                new PromoCode
                {
                    Code = "SAVE20",
                    DiscountType = DiscountType.FixedAmount,
                    DiscountValue = 20,
                    MinOrderAmount = 100,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1),
                    UsageLimit = 50,
                    TimesUsed = 5,
                    IsActive = true
                },
                new PromoCode
                {
                    Code = "EXPIRED5",
                    DiscountType = DiscountType.Percentage,
                    DiscountValue = 5,
                    MinOrderAmount = 0,
                    ExpiryDate = DateTime.UtcNow.AddDays(-10),
                    UsageLimit = 0,
                    TimesUsed = 0,
                    IsActive = true
                },
                new PromoCode
                {
                    Code = "MAXEDOUT",
                    DiscountType = DiscountType.Percentage,
                    DiscountValue = 15,
                    MinOrderAmount = 0,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1),
                    UsageLimit = 10,
                    TimesUsed = 10,
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedResourceStringsAsync(AppDbContext context)
        {
            if (await context.ResourceStrings.AnyAsync())
                return;

            var en = await context.Languages.FirstAsync(l => l.Code == "en");
            var ar = await context.Languages.FirstAsync(l => l.Code == "ar");

            context.ResourceStrings.AddRange(
                new ResourceString { Key = "order.confirmed.subject", LanguageId = en.Id, Value = "Your order has been confirmed" },
                new ResourceString { Key = "order.confirmed.subject", LanguageId = ar.Id, Value = "تم تأكيد طلبك" },
                new ResourceString { Key = "order.shipped.subject", LanguageId = en.Id, Value = "Your order has shipped" },
                new ResourceString { Key = "order.shipped.subject", LanguageId = ar.Id, Value = "تم شحن طلبك" },
                new ResourceString { Key = "welcome.message", LanguageId = en.Id, Value = "Welcome to Elara!" },
                new ResourceString { Key = "welcome.message", LanguageId = ar.Id, Value = "!مرحبا بك في إيلارا" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedUserDependentDataAsync(AppDbContext context)
        {
            var users = await context.Users.Take(2).ToListAsync();

            if (users.Count < 2)
            {
                // Not enough real users yet — skip. Seed these manually once
                // Auth & Setup has real accounts in the Users table.
                return;
            }

            var userA = users[0];
            var userB = users[1];

            if (!await context.LoyaltyTransactions.AnyAsync())
            {
                context.LoyaltyTransactions.AddRange(
                    new Transaction { UserId = userA.Id, OrderId = null, Points = 100, Type = TransactionType.Earned, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                    new Transaction { UserId = userA.Id, OrderId = null, Points = 30, Type = TransactionType.Redeemed, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                    new Transaction { UserId = userA.Id, OrderId = null, Points = 50, Type = TransactionType.Earned, CreatedAt = DateTime.UtcNow.AddDays(-1) }
                );
            }

            if (!await context.Referrals.AnyAsync())
            {
                context.Referrals.Add(new Referral
                {
                    ReferrerUserId = userA.Id,
                    ReferredUserId = userB.Id,
                    ReferralCode = "REFSEEDTEST",
                    Status = ReferralStatus.Completed,
                    RewardPoints = 50,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                });
            }

            if (!await context.NewsletterSubscriptions.AnyAsync())
            {
                context.NewsletterSubscriptions.AddRange(
                    new NewsletterSubscription { UserId = userA.Id, Email = "usera-test@example.com", IsSubscribed = true, SubscribedAt = DateTime.UtcNow.AddDays(-20) },
                    new NewsletterSubscription { UserId = null, Email = "guest-test@example.com", IsSubscribed = true, SubscribedAt = DateTime.UtcNow.AddDays(-2) }
                );
            }

            if (!await context.DeviceTokens.AnyAsync())
            {
                context.DeviceTokens.Add(new DeviceToken
                {
                    UserId = userA.Id,
                    Token = "test-device-token-abc123",
                    Platform = DevicePlatform.Android,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }
    }
}