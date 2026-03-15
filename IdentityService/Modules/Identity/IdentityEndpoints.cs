using FluentValidation;
using FluentValidation.Results;
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

    private static async Task<Results<Created, BadRequest<string>>> Register(
        RegisterDto dto,
        DatabaseContext context,
        IPasswordHasher passwordHasher,
        IValidator<RegisterDto> validator
    )
    {
        await validator.ValidateAndThrowAsync(dto);

        var email = new Email(dto.Email);

        var exists = await context.Users.AnyAsync(u => u.Email == email.Address);
        if (exists)
        {
            throw new ValidationException([
                new ValidationFailure("message", "Something wrong.")
            ]);
        }

        var strategy = context.Database.CreateExecutionStrategy();
        var createdUserId = Guid.Empty;

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            var user = new User(
                Guid.Empty,
                dto.Name,
                email,
                dto.Password,
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

            createdUserId = user.Id;
        });

        return TypedResults.Created($"/users/{createdUserId}");
    }

    // for more than one type of return, we need to use: Results<Ok<string>, BadRequest<string>>
    private static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> Login(
        LoginDto loginDto,
        TokenProvider tokenProvider,
        DatabaseContext db,
        IPasswordHasher passwordHasher
        )
    {
        var email = new Email(loginDto.Email);

        // Busca do banco
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email.Address);
        if (user is null)
            throw new UnauthorizedAccessException();

        // Valida senha
        if (!passwordHasher.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException();

        // (opcional) bloquear login se não verificado
        // if (!user.IsVerified) return TypedResults.BadRequest("Email não verificado.");

        var response = new LoginResponseDto(
            await tokenProvider.Create(user),
            tokenProvider.ExpiresIn()
        );

        return TypedResults.Ok(response);
    }

    private static Ok<string> VerifyEmail()
    {
        return TypedResults.Ok("ok");
    }

    private static Ok ResetPassword(IConfiguration configuration)
    {
        return TypedResults.Ok();
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