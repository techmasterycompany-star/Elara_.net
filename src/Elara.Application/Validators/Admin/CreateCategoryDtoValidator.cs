using Elara.Application.DTOs.Category;
using Elara.Application.Interfaces.Repository;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator(ICategoryRepository categoryRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.ParentCategoryId)
                .GreaterThan(0).WithMessage("Parent category ID must be greater than 0.")
                .When(x => x.ParentCategoryId.HasValue);
        }
    }
}
