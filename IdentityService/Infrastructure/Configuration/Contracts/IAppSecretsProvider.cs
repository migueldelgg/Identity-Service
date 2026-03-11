using IdentityService.Infrastructure.Configuration.Models;

namespace IdentityService.Infrastructure.Configuration.Contracts;

public interface IAppSecretsProvider
{
    AppSecrets Load();
}