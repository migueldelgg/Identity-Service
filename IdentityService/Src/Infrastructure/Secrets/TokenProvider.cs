using System.Security.Claims;
using System.Security.Cryptography;
using IdentityService.Infrastructure.Database;
using IdentityService.Modules.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace IdentityService.Infrastructure.Secrets;

internal sealed class TokenProvider(SecretsSettings secrets, DatabaseContext dbContext)
{
    public async Task<string> Create(User user)
    {
        // 1) Carrega a PRIVATE KEY (PEM) do SecretsSettings
        var signingKey = BuildRsaKey(secrets.JwtPrivateKeyPem);
        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.RsaSha256
        );
        
        // 2) Busca roles do usuário
        List<string> roleNames = await dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        // 3) Monta claims
        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.EmailVerified, user.IsVerified ? "true" : "false"),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
        ];
        
        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

        // 4) Descritor + criação do token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(secrets.JwtExpiriesInMinutes),
            SigningCredentials = credentials,
            Issuer = secrets.JwtIssuer,
            Audience = secrets.JwtAudience,
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }
    
    public static RsaSecurityKey BuildRsaKey(string pemKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pemKey);
        return new RsaSecurityKey(rsa);
    }
}