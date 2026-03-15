namespace IdentityService.Modules.Identity.UseCases;

public sealed record LoginDto(string Email, string Password);

public sealed record LoginResponseDto(string Token, int ExpiresInMinutes);