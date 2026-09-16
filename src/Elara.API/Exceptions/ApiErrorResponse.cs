namespace Elara.API.Exceptions;

using Microsoft.AspNetCore.Mvc;

public sealed class ErrorResponse
{
    public bool Success { get; set; }
    public ProblemDetails Error { get; set; } = null!;
}