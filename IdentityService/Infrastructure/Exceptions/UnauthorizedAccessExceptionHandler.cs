using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Infrastructure.Exceptions;

public class UnauthorizedAccessExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<UnauthorizedAccessExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedAccessException unauthorizedAccessException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
        };

        return await problemDetailsService.TryWriteAsync(context);
    }
}