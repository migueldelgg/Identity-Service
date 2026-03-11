namespace IdentityService.Infrastructure.Configuration.Models;

public sealed class AppSecrets
{
    public string PostgreSqlConnection { get; init; } = string.Empty;
    
    public string JwtIssuer { get; init; } = string.Empty;
    public string JwtAudience { get; init; } = string.Empty;
    public int JwtExpiresMinutes { get; init; }
    public string JwtPrivateKey { get; init; } = string.Empty;
    public string JwtPublicKey { get; init; } = string.Empty;
}