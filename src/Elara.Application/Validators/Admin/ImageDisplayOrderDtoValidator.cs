using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class ImageDisplayOrderDtoValidator : AbstractValidator<ImageDisplayOrderDto>
    {
        public ImageDisplayOrderDtoValidator()
        {
            RuleFor(x => x.ImageId)
                .GreaterThan(0).WithMessage("Image ID must be greater than 0.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
        }
    }
}
