using Elara.Application.DTOs.ShippingMethods;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateShippingMethodStatusDtoValidator : AbstractValidator<UpdateShippingMethodStatusDto>
    {
        public UpdateShippingMethodStatusDtoValidator()
        {
        }
    }
}
