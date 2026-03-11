using IdentityService.Infrastructure.Configuration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
        {
            var secrets = serviceProvider.GetRequiredService<AppSecrets>();

            options.UseNpgsql(
                secrets.PostgreSqlConnection,
                npgsql =>
                {
                    npgsql.EnableRetryOnFailure(3);
                    npgsql.CommandTimeout(30);
                });

            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        return services;
    }
}