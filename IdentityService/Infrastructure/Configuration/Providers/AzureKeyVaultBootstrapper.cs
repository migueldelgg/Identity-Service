using Azure.Identity;
using IdentityService.Infrastructure.Configuration.Contracts;

namespace IdentityService.Infrastructure.Configuration.Providers;

public sealed class AzureKeyVaultBootstrapper : IConfigurationProviderBootstrapper
{
    public void Bootstrap(WebApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];

        if (string.IsNullOrWhiteSpace(keyVaultUri))
            throw new InvalidOperationException("Configuração obrigatória ausente: 'KeyVault:VaultUri'.");

        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential()
        );
    }
}