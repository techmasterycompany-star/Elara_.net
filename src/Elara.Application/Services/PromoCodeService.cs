using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Elara.Domain.Enums;

namespace Elara.Application.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public PromoCodeService(IPromoCodeRepository promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task<PromoCodeValidationResultDto> ValidateAsync(
            string code,
            decimal orderSubTotal,
            CancellationToken cancellationToken = default)
        {
            var promoCode = await _promoCodeRepository.GetByCodeAsync(code, cancellationToken);

            if (promoCode == null)
                return Invalid("PROMO_NOT_FOUND", "This promo code does not exist.");

            if (!promoCode.IsActive)
                return Invalid("PROMO_INACTIVE", "This promo code is no longer active.");

            if (promoCode.ExpiryDate < DateTime.UtcNow)
                return Invalid("PROMO_EXPIRED", "This promo code has expired.");

            if (promoCode.UsageLimit > 0 && promoCode.TimesUsed >= promoCode.UsageLimit)
                return Invalid("PROMO_LIMIT_REACHED", "This promo code has reached its usage limit.");

            if (orderSubTotal < promoCode.MinOrderAmount)
                return Invalid("PROMO_MIN_ORDER_NOT_MET",
                    $"A minimum order of {promoCode.MinOrderAmount:C} is required to use this code.");

            var discount = promoCode.DiscountType == DiscountType.Percentage
                ? orderSubTotal * (promoCode.DiscountValue / 100m)
                : promoCode.DiscountValue;

            discount = Math.Min(discount, orderSubTotal);

            return new PromoCodeValidationResultDto
            {
                IsValid = true,
                DiscountAmount = discount,
                FinalTotal = orderSubTotal - discount
            };
        }

        private static PromoCodeValidationResultDto Invalid(string errorCode, string message) => new()
        {
            IsValid = false,
            ErrorCode = errorCode,
            ErrorMessage = message
        };

        public async Task<PromoCodeDetailsDto?> GetByCodeAsync(
    string code,
    CancellationToken cancellationToken = default)
        {
            var promoCode = await _promoCodeRepository.GetByCodeAsync(code, cancellationToken);

            if (promoCode == null)
                return null;

            return new PromoCodeDetailsDto
            {
                Code = promoCode.Code,
                DiscountType = promoCode.DiscountType.ToString(),
                DiscountValue = promoCode.DiscountValue,
                MinOrderAmount = promoCode.MinOrderAmount,
                ExpiryDate = promoCode.ExpiryDate,
                IsActive = promoCode.IsActive
            };
        }
    }
}