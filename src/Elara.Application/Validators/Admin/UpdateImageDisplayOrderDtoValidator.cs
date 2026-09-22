using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateImageDisplayOrderDtoValidator : AbstractValidator<UpdateImageDisplayOrderDto>
    {
        public UpdateImageDisplayOrderDtoValidator()
        {
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
        }
    }
}
