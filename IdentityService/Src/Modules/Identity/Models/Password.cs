using IdentityService.Modules.Identity.UseCases.PasswordHashing;

namespace IdentityService.Modules.Identity.Models;

internal sealed class Password
{
    public string Hash { get; }

    public Password(string password, IPasswordHasher passwordHasher)
    {
        Hash = passwordHasher.Hash(password);
    }

    public bool Verify(string password, string userPasswordHash, IPasswordHasher passwordHasher)
    {
        return passwordHasher.Verify(userPasswordHash, password);
    }
}