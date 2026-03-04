namespace IdentityService.Modules.Identity.Models;

internal class Token
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Jwt { get; }
    public DateTime ExpiresAt { get; }

    public User User { get; }

    public Token() { }

    public Token(Guid id, Guid userId, string jwt, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        Jwt = jwt;
        ExpiresAt = expiresAt;
    }
}