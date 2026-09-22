using Elara.Application.DTOs.Order;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateOrderStatusRequestDtoValidator : AbstractValidator<UpdateOrderStatusRequestDto>
    {
        public UpdateOrderStatusRequestDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Notes));
        }
    }
}
