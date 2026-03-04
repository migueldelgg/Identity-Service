using IdentityService.Modules.Identity.UseCases.PasswordHashing;

namespace IdentityService.Modules.Identity.Models;

internal class User
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; init; }
    public bool IsVerified { get; init; }

    public User() { }

    public User(
        Guid id,
        string name,
        Email email,
        string plainPassword,
        bool isVerified,
        IPasswordHasher passwordHasher
    )
    {
        Id = id;
        Name = name;
        Email = email.Address;
        PasswordHash = new Password(plainPassword, passwordHasher).Hash;
        IsVerified = isVerified;
    }
}