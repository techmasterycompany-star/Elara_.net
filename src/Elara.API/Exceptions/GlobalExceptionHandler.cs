using Elara.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Elara.API.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService problemDetails;
        public GlobalExceptionHandler(IProblemDetailsService problemDetails) => this.problemDetails = problemDetails;


        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
                BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
            };

            context.Response.StatusCode = status;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ? exception.Message : null,
                Instance = context.Request.Path
            };

            var response = new ErrorResponse
            {
                Success = false,
                Error = problem
            };

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            return true;
        }

    }
}
