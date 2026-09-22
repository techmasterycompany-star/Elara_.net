using Elara.Application.DTOs.Order;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class AdminOrderFilterDtoValidator : AbstractValidator<AdminOrderFilterDto>
    {
        public AdminOrderFilterDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage("SortBy must be one of: Id, OrderDate, TotalAmount, Status.");

            RuleFor(x => x.SortOrder)
                .IsInEnum().WithMessage("SortOrder must be Asc or Desc.")
                .When(x => x.SortOrder.HasValue);

            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status.")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate)
                .WithMessage("From date must be earlier than or equal to To date.")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

            RuleFor(x => x.SellerId)
                .GreaterThan(0).WithMessage("Seller ID must be greater than 0.")
                .When(x => x.SellerId.HasValue);

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer ID must be greater than 0.")
                .When(x => x.CustomerId.HasValue);
        }
    }
}
