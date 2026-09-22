using Elara.Application.DTOs.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.Validators.Seller
{
    public class SellerOrderQueryValidator : AbstractValidator<SellerOrderQuery>
    {
        public SellerOrderQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x.SortOrder)
                .IsInEnum().WithMessage("SortOrder must be Asc or Desc.")
                .When(x => x.SortOrder.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status.")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.DateFrom)
                .LessThanOrEqualTo(x => x.DateTo)
                .WithMessage("Date from must be earlier than or equal to Date to.")
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
        }
    }
}
