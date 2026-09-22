using Elara.Application.DTOs.HomePageContent;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateBannerStatusDtoValidator : AbstractValidator<UpdateBannerStatusDto>
    {
        public UpdateBannerStatusDtoValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive is required.");
        }
    }
}
