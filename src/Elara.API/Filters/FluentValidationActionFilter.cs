using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Elara.API.Filters
{
    public class FluentValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider serviceProvider;

        public FluentValidationActionFilter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.Value.GetType());
                var validator = serviceProvider.GetService(validatorType) as IValidator;

                if (validator == null) continue;

                var validationContext = new ValidationContext<object>(argument.Value);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        errors = result.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                    return;
                }
            }

            await next();
        }
    }
}
