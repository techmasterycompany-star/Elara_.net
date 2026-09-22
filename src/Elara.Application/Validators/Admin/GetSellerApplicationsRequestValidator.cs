using Elara.Application.DTOs.SellerApplication;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class GetSellerApplicationsRequestValidator : AbstractValidator<GetSellerApplicationsRequest>
    {
        public GetSellerApplicationsRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

            RuleFor(x => x.SellerId)
                .GreaterThan(0).WithMessage("Seller ID must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid seller application status.")
                .When(x => x.Status.HasValue);
        }
    }
}
