using Elara.Application.DTOs.SellerApplication;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class RejectSellerApplicationDtoValidator : AbstractValidator<RejectSellerApplicationDto>
    {
        public RejectSellerApplicationDtoValidator()
        {
            RuleFor(x => x.RejectionReason)
                .NotEmpty().WithMessage("Rejection reason is required.")
                .MaximumLength(500).WithMessage("Rejection reason must not exceed 500 characters.");
        }
    }
}
