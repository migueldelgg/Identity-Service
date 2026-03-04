namespace IdentityService.Modules.Identity.UseCases;

public record RegisterRequest(string Name, string Email, string Password);