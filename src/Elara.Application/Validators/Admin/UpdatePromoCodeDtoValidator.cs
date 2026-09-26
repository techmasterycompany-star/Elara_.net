using Elara.Application.DTOs;
using Elara.Domain.Enums;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdatePromoCodeDtoValidator : AbstractValidator<UpdatePromoCodeDto>
    {
        public UpdatePromoCodeDtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Promo code is required.")
                .MaximumLength(50).WithMessage("Promo code must not exceed 50 characters.")
                .Matches("^[A-Za-z0-9_-]+$").WithMessage("Promo code can only contain letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.DiscountType)
                .IsInEnum().WithMessage("Invalid discount type.");

            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Discount value must be greater than 0.");

            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100)
                .When(x => x.DiscountType == DiscountType.Percentage)
                .WithMessage("Percentage discount cannot exceed 100%.");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount cannot be negative.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be greater than 0.");
        }
    }
}
