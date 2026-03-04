namespace IdentityService.Modules.Identity.UseCases;

public sealed record LoginRequest(string Email, string Password);