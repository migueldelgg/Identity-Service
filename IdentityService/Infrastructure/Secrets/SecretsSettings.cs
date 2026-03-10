namespace IdentityService.Infrastructure.Secrets;

public sealed class SecretsSettings
{
    public string JwtPrivateKeyPem { get; }
    public string JwtPublicKeyPem { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int JwtExpiriesInMinutes { get; }

    private SecretsSettings(
        string jwtPrivateKeyPem,
        string jwtPublicKeyPem,
        string jwtIssuer,
        string jwtAudience,
        int jwtExpiresMinutes)
    {
        JwtPrivateKeyPem = jwtPrivateKeyPem;
        JwtPublicKeyPem = jwtPublicKeyPem;
        JwtIssuer = jwtIssuer;
        JwtAudience = jwtAudience;
        JwtExpiriesInMinutes = jwtExpiresMinutes;
    }

    public static SecretsSettings FromEnvironment(IConfiguration config)
    {
        var privateKeyPath = Require(config["JWT_PRIVATE_KEY_PATH"], "JWT_PRIVATE_KEY_PATH");
        var publicKeyPath  = Require(config["JWT_PUBLIC_KEY_PATH"], "JWT_PUBLIC_KEY_PATH");

        if (!File.Exists(privateKeyPath))
            throw new InvalidOperationException($"Private key não encontrada em: {privateKeyPath}");

        if (!File.Exists(publicKeyPath))
            throw new InvalidOperationException($"Public key não encontrada em: {publicKeyPath}");

        var privateKeyPem = File.ReadAllText(privateKeyPath);
        var publicKeyPem  = File.ReadAllText(publicKeyPath);

        var jwtIssuer = Require(config["JWT_ISSUER"], "JWT_ISSUER");
        var jwtAudience = Require(config["JWT_AUDIENCE"], "JWT_AUDIENCE");

        var jwtExpiresMinutesRaw = Require(config["JWT_EXPIRES_MINUTES"], "JWT_EXPIRES_MINUTES");

        if (!int.TryParse(jwtExpiresMinutesRaw, out var jwtExpiresMinutes))
            throw new InvalidOperationException(
                $"Variável JWT_EXPIRES_MINUTES inválida: '{jwtExpiresMinutesRaw}' (deve ser int).");

        return new SecretsSettings(
            privateKeyPem,
            publicKeyPem,
            jwtIssuer,
            jwtAudience,
            jwtExpiresMinutes);
    }

    private static string Require(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                $"Variável de ambiente obrigatória não encontrada: {name}");

        return value.Trim();
    }
}