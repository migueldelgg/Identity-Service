namespace IdentityService.Infrastructure.Configuration.Contracts;

public interface IConfigurationProviderBootstrapper
{
    void Bootstrap(WebApplicationBuilder builder);
}