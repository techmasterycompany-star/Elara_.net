using Elara.Application.DTOs.Product;
using FluentValidation;

namespace Elara.Application.Validators.Admin
{
    public class UpdateProductStatusDtoValidator : AbstractValidator<UpdateProductStatusDto>
    {
        public UpdateProductStatusDtoValidator()
        {
        }
    }
}
