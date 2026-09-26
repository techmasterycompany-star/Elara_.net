using Elara.Application.DTOs.Shipment;
using FluentValidation;

namespace Elara.Application.Validators.Seller
{
    public class ShipSellerShipmentDtoValidator : AbstractValidator<ShipSellerShipmentDto>
    {
        public ShipSellerShipmentDtoValidator()
        {
            RuleFor(x => x.Carrier)
                .NotEmpty().WithMessage("Carrier is required.")
                .MaximumLength(100).WithMessage("Carrier must not exceed 100 characters.");

            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("Tracking number is required.")
                .MaximumLength(100).WithMessage("Tracking number must not exceed 100 characters.");

            RuleFor(x => x.EstimatedDeliveryDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.EstimatedDeliveryDate.HasValue)
                .WithMessage("Estimated delivery date must be in the future.");
        }
    }
}
