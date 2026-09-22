using Elara.Application.DTOs.ShippingMethods;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateShippingMethodDtoValidator : AbstractValidator<UpdateShippingMethodDto>
    {
        public UpdateShippingMethodDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Shipping method name is required.")
                .MaximumLength(100).WithMessage("Shipping method name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Shipping method description is required.")
                .MaximumLength(500).WithMessage("Shipping method description must not exceed 500 characters.");

            RuleFor(x => x.BaseCost)
                .GreaterThanOrEqualTo(0).WithMessage("Base cost cannot be negative.");

            RuleFor(x => x.EstimatedDays)
                .GreaterThan(0).WithMessage("Estimated days must be greater than 0.");
        }
    }
}
