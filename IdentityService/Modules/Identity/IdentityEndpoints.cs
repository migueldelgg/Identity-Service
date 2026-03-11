using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using IdentityService.Infrastructure.Authentication;
using IdentityService.Infrastructure.Database;
using IdentityService.Modules.Identity.Models;
using IdentityService.Modules.Identity.Models.Role;
using IdentityService.Modules.Identity.UseCases;
using IdentityService.Modules.Identity.UseCases.PasswordHashing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Modules.Identity;

internal static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/identity");

        group.MapPost("/register", Register).WithName(nameof(Register));
        group.MapPost("/login", Login).WithName(nameof(Login));

        group.MapPost("/verify-email", VerifyEmail).WithName(nameof(VerifyEmail));

        group.MapPost("/reset-password", ResetPassword)
            .WithName(nameof(ResetPassword))
            .RequireAuthorization(policy => policy.RequireRole(Role.Admin, Role.Member));

        group.MapPost("/new-password", NewPassword).WithName(nameof(NewPassword));
        group.MapPost("/oauth", Oauth).WithName(nameof(Oauth));
        group.MapPost("/callback", Callback).WithName(nameof(Callback));

        return app;
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> Register(
        RegisterRequest request,
        DatabaseContext context,
        IPasswordHasher passwordHasher
    )
    {
        var email = new Email(request.Email);

        var exists = await context.Users.AnyAsync(u => u.Email == email.Address);
        if (exists) return TypedResults.BadRequest("Email já cadastrado.");

        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            var user = new User(
                Guid.Empty,
                request.Name,
                email,
                request.Password,
                false,
                passwordHasher
            );

            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = Role.MemberId
            });

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        });

        return TypedResults.Ok("Usuário criado");
    }

    // for more than one type of return, we need to use: Results<Ok<string>, BadRequest<string>>
    private static async Task<Results<Ok<string>, UnauthorizedHttpResult, BadRequest<string>>> Login(
        LoginRequest loginRequest,
        TokenProvider tokenProvider,
        DatabaseContext db,
        IPasswordHasher passwordHasher
        )
    {
        var email = new Email(loginRequest.Email);

        // Busca do banco
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email.Address);
        if (user is null)
            return TypedResults.Unauthorized();

        // Valida senha
        if (!passwordHasher.Verify(loginRequest.Password, user.PasswordHash))
            return TypedResults.Unauthorized();

        // (opcional) bloquear login se não verificado
        // if (!user.IsVerified) return TypedResults.BadRequest("Email não verificado.");

        var token = await tokenProvider.Create(user);

        return TypedResults.Ok(token);
    }

    private static Ok<string> VerifyEmail()
    {
        return TypedResults.Ok("ok");
    }

    private static async Task<Ok<KeyVaultSecret>> ResetPassword(IConfiguration configuration)
    {
        var secretsClient = new SecretClient(
            new Uri(configuration["KeyVault:VaultUri"]!),
            new DefaultAzureCredential());

        Response<KeyVaultSecret> response = await secretsClient.GetSecretAsync("PostgresConnectionString");
        return TypedResults.Ok(response.Value);
    }

    private static Ok<string> NewPassword()
    {
        return TypedResults.Ok("ok");
    }   

    private static Ok<string> Oauth()
    {
        return TypedResults.Ok("ok");
    }

    private static Ok<string> Callback()
    {
        return TypedResults.Ok("ok");
    }
}