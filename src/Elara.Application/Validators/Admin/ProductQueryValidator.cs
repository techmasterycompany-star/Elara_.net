using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class ProductQueryValidator : AbstractValidator<ProductQuery>
    {
        public ProductQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage("SortBy must be one of: Name, Price, StockQuantity, CreatedAt, UpdatedAt.");

            RuleFor(x => x.SortOrder)
                .IsInEnum().WithMessage("SortOrder must be Asc or Desc.")
                .When(x => x.SortOrder.HasValue);

            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.")
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.SellerProfileId)
                .GreaterThan(0).WithMessage("Seller profile ID must be greater than 0.")
                .When(x => x.SellerProfileId.HasValue);
        }
    }
}
