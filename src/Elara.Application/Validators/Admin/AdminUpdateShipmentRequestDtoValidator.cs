using Elara.Application.DTOs.Shipment;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class AdminUpdateShipmentRequestDtoValidator : AbstractValidator<AdminUpdateShipmentRequestDto>
    {
        public AdminUpdateShipmentRequestDtoValidator()
        {
            RuleFor(x => x.Carrier)
                .MaximumLength(100).WithMessage("Carrier must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Carrier));

            RuleFor(x => x.TrackingNumber)
                .MaximumLength(100).WithMessage("Tracking number must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.TrackingNumber));

            RuleFor(x => x.EstimatedDeliveryDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Estimated delivery date cannot be in the past.")
                .When(x => x.EstimatedDeliveryDate.HasValue);
        }
    }
}
