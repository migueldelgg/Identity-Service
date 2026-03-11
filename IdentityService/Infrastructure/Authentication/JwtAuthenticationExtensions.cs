using IdentityService.Infrastructure.Configuration.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Infrastructure.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<SecurityKey>(serviceProvider =>
        {
            var secrets = serviceProvider.GetRequiredService<AppSecrets>();
            return TokenProvider.BuildRsaKey(secrets.JwtPublicKey);
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<AppSecrets, SecurityKey>((options, secrets, signingKey) =>
            {
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = secrets.JwtIssuer,
                    ValidAudience = secrets.JwtAudience,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = signingKey
                };
            });

        return services;
    }
}