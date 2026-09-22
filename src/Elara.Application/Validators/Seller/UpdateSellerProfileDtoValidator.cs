using Elara.Application.DTOs.SellerProfile;
using FluentValidation;

namespace Elara.Application.Validators.Seller
{
    public class UpdateSellerProfileDtoValidator : AbstractValidator<UpdateSellerProfileDto>
    {
        public UpdateSellerProfileDtoValidator()
        {
            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("Store name is required.")
                .MaximumLength(100).WithMessage("Store name must not exceed 100 characters.");

            RuleFor(x => x.StoreDescription)
                .NotEmpty().WithMessage("Store description is required.")
                .MaximumLength(1000).WithMessage("Store description must not exceed 1000 characters.");
        }
}
}
