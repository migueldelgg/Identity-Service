namespace IdentityService.Infrastructure.Exceptions;

public static class RegisterExceptions
{
    
    /// <summary>
    /// Registra o tratamento global de exceções da aplicação.
    ///
    /// - Habilita ProblemDetails (RFC 7807)
    /// - Adiciona `requestId` nas respostas de erro
    /// - Registra o GlobalExceptionHandler
    ///
    /// IMPORTANTE:
    /// A ordem dos ExceptionHandlers importa.
    /// Handlers específicos devem ser registrados antes,
    /// deixando o GlobalExceptionHandler como fallback final.
    /// </summary>
    public static IServiceCollection AddExceptionConfiguration(this IServiceCollection services)
    {
        services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = options =>
            {
                options.ProblemDetails.Extensions.TryAdd("requestId", options.HttpContext.TraceIdentifier);
            };
        });
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<UnauthorizedAccessExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}