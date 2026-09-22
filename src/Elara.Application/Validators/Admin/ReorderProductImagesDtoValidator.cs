using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class ReorderProductImagesDtoValidator : AbstractValidator<ReorderProductImagesDto>
    {
        public ReorderProductImagesDtoValidator()
        {
            RuleFor(x => x.Images)
                .NotEmpty().WithMessage("At least one image is required.");

            RuleFor(x => x.Images)
                .Must(images => images.Select(x => x.ImageId).Distinct().Count() == images.Count)
                .WithMessage("Duplicate image IDs are not allowed.");

            RuleFor(x => x.Images)
                .Must(images => images.Select(x => x.DisplayOrder).Distinct().Count() == images.Count)
                .WithMessage("Duplicate display orders are not allowed.");

            RuleForEach(x => x.Images)
                .SetValidator(new ImageDisplayOrderDtoValidator());
        }
    }
}
