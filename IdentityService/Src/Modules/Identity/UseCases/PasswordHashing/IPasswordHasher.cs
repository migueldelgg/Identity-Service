namespace IdentityService.Modules.Identity.UseCases.PasswordHashing;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Verify(string password, string userPasswordHash);
}