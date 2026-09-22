using Elara.Application.DTOs.Shipment;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateShipmentStatusRequestDtoValidator : AbstractValidator<UpdateShipmentStatusRequestDto>
    {
        public UpdateShipmentStatusRequestDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid shipment status.");
        }
    }
}
