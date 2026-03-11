using IdentityService.Infrastructure.Configuration.Contracts;
using IdentityService.Infrastructure.Configuration.Models;
using IdentityService.Infrastructure.Configuration.Providers;

namespace IdentityService.Infrastructure.Configuration.Extensions;

public static class ConfigurationRegistrationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAppSecretsProvider, ConfigurationAppSecretsProvider>();

        services.AddSingleton<AppSecrets>(serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<IAppSecretsProvider>();
            return provider.Load();
        });

        return services;
    }

    public static WebApplicationBuilder BootstrapExternalConfiguration(this WebApplicationBuilder builder)
    {
        IConfigurationProviderBootstrapper bootstrapper = new AzureKeyVaultBootstrapper();
        bootstrapper.Bootstrap(builder);

        return builder;
    }
}