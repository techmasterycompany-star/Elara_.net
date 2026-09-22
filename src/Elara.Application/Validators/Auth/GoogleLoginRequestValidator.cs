using Elara.Application.DTOs.Auth;
using FluentValidation;

namespace Elara.Application.Validators.Auth
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID token is required.");
        }
    }
}
