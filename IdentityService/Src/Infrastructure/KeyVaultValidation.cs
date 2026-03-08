namespace IdentityService.Infrastructure;

public class AppSecrets
{
    public string PostgreSqlConnection { get; init; } = string.Empty;

    public string JwtIssuer { get; init; } = string.Empty;

    public string JwtAudience { get; init; } = string.Empty;

    public int JwtExpiresMinutes { get; init; } = 0;

    public string JwtPrivateKeyPem { get; init; } = string.Empty;

    public string JwtPublicKeyPem { get; init; } = string.Empty;
}

// TODO: Adicionar tratamento de exception System.InvalidOperationException:
public static class KeyVaultValidation
{
    public static void LoadAndRegister(WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        var secrets = new AppSecrets
        {
            PostgreSqlConnection = Require(config, "ConnectionStrings:PostgreSql"),
            JwtIssuer = Require(config, "Jwt:Issuer"),
            JwtAudience = Require(config, "Jwt:Audience"),
            JwtExpiresMinutes = int.Parse(Require(config, "Jwt:ExpiresMinutes")),
            // JwtPrivateKeyPem = Require(config, "Jwt:PrivateKeyPem"),
            // JwtPublicKeyPem = Require(config, "Jwt:PublicKeyPem")
        };

        builder.Services.AddSingleton(secrets);
    }

    private static string Require(IConfiguration config, string key)
    {
        var value = config[key];
        
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configuração obrigatória ausente ou vazia: '{key}'.");
        
        return value;
    }
}