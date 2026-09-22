using Elara.Application.DTOs.Auth;
using Elara.Application.Interfaces.Repository;
using FluentValidation;

namespace Elara.Application.Validators.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
                .MustAsync(async (username, cancellation) =>
                {
                    var existing = await userRepository.GetByUsernameAsync(username);
                    return existing == null;
                }).WithMessage("Username is already taken.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MustAsync(async (email, cancellation) =>
                {
                    var existing = await userRepository.GetByEmailAsync(email);
                    return existing == null;
                }).WithMessage("Email is already registered.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Role must be one of: Admin(1), Seller(2), Customer(3).");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .MustAsync(async (phone, cancellation) =>
                {
                    var existing = await userRepository.GetByPhoneNumberAsync(phone);
                    return existing == null;
                }).WithMessage("Phone number is already registered.");
        }
    }
}
