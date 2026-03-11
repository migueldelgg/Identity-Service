using IdentityService.Infrastructure.Configuration.Contracts;
using IdentityService.Infrastructure.Configuration.Models;

namespace IdentityService.Infrastructure.Configuration.Providers;

public sealed class ConfigurationAppSecretsProvider : IAppSecretsProvider
{
    private readonly IConfiguration _configuration;

    public ConfigurationAppSecretsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppSecrets Load()
    {
        return new AppSecrets
        {
            PostgreSqlConnection = Require("ConnectionStrings:PostgreSql"),
            JwtIssuer = Require("Jwt:Issuer"),
            JwtAudience = Require("Jwt:Audience"),
            JwtExpiresMinutes = ParseInt("Jwt:ExpiresMinutes"),
            JwtPrivateKey = Require("Jwt:PrivateKey"),
            JwtPublicKey = Require("Jwt:PublicKey")
        };
    }

    private string Require(string key)
    {
        var value = _configuration[key];

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configuração obrigatória ausente ou vazia: '{key}'.");

        return value;
    }

    private int ParseInt(string key)
    {
        var raw = Require(key);

        if (!int.TryParse(raw, out var value))
            throw new InvalidOperationException($"Configuração inválida para '{key}'. Valor recebido: '{raw}'.");

        return value;
    }
}