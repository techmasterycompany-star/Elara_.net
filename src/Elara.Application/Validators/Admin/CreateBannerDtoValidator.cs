using Elara.Application.DTOs.HomePageContent;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class CreateBannerDtoValidator : AbstractValidator<CreateBannerDto>
    {
        public CreateBannerDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Banner title is required.")
                .MaximumLength(200).WithMessage("Banner title must not exceed 200 characters.");

            RuleFor(x => x.Subtitle)
                .NotEmpty().WithMessage("Banner subtitle is required.")
                .MaximumLength(500).WithMessage("Banner subtitle must not exceed 500 characters.");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Banner image is required.")
                .Must(file => file == null || file.Length > 0)
                .WithMessage("Banner image cannot be empty.");

            RuleFor(x => x.Image)
                .Must(file => file == null || new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                .WithMessage("Banner image must be JPG, JPEG, PNG, or WEBP.");

            RuleFor(x => x.LinkUrl)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid link URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.LinkUrl));

            RuleFor(x => x.Position)
                .IsInEnum().WithMessage("Invalid banner position.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be later than start date.")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        }
    }
}
