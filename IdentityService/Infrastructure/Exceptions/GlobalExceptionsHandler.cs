using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService, 
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    // a ordem de injecao de dependencias dos exceptions handler importa
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exceptions occurred.");

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext =  httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails {
                Type= exception.GetType().Name,
                Title = "An error occurred while processing the request.",
                Detail = exception.Message
            }
        });
    }
}