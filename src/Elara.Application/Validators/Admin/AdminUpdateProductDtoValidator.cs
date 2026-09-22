using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class AdminUpdateProductDtoValidator : AbstractValidator<AdminUpdateProductDto>
    {
        public AdminUpdateProductDtoValidator()
        {
            RuleFor(x => x.SellerProfileId)
                .GreaterThan(0).WithMessage("Seller profile ID must be greater than 0.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(2000).WithMessage("Product description must not exceed 2000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than 0.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }
    }
}
