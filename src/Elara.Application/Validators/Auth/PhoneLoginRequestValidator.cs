using Elara.Application.DTOs.Auth;
using FluentValidation;

namespace Elara.Application.Validators.Auth
{
    public class PhoneLoginRequestValidator : AbstractValidator<PhoneLoginRequest>
    {
        public PhoneLoginRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required.")
                .Length(6).WithMessage("OTP must be exactly 6 digits.")
                .Matches("^[0-9]+$").WithMessage("OTP must contain only digits.");
        }
    }
}
