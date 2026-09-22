using Elara.Application.DTOs.Order;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class GetMyOrdersRequestValidator : AbstractValidator<GetMyOrdersRequest>
    {
        public GetMyOrdersRequestValidator()
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
        }
    }
}
