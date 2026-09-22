using Elara.Application.DTOs.Category;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class CategoryListRequestValidator : AbstractValidator<CategoryListRequest>
    {
        public CategoryListRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x.SortOrder)
                .IsInEnum().WithMessage("SortOrder must be Asc or Desc.")
                .When(x => x.SortOrder.HasValue);

            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0).WithMessage("Parent category ID must be greater than 0.")
                .When(x => x.ParentCategoryId.HasValue);
        }
    }
}
