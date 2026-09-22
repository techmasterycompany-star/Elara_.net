using Elara.Application.DTOs.HomePageContent;
using Elara.Domain.Enums;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateHomepageSectionDtoValidator : AbstractValidator<UpdateHomepageSectionDto>
    {
        public UpdateHomepageSectionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Section title is required.")
                .MaximumLength(200).WithMessage("Section title must not exceed 200 characters.");

            RuleFor(x => x.SubTitle)
                .MaximumLength(500).WithMessage("Section subtitle must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.SubTitle));

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid homepage section type.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

            RuleFor(x => x.MaxItems)
                .GreaterThan(0).WithMessage("Max items must be greater than 0.");

            RuleFor(x => x.BannerId)
                .GreaterThan(0).WithMessage("Banner ID must be greater than 0.")
                .When(x => x.BannerId.HasValue);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x)
                .Must(x => x.Type != HomepageSectionType.Banner || x.BannerId.HasValue)
                .WithMessage("Banner ID is required for Banner sections.");
            
            RuleFor(x => x)
                .Must(x => x.Type != HomepageSectionType.CategoryProducts || x.CategoryId.HasValue)
                .WithMessage("Category ID is required for Category Products sections.");
        }
    }
}
