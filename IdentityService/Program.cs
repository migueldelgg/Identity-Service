using IdentityService.Infrastructure.Authentication;
using IdentityService.Infrastructure.Configuration.Extensions;
using IdentityService.Infrastructure.Database;
using IdentityService.Modules.Identity;
using IdentityService.Modules.Identity.UseCases.PasswordHashing;

var builder = WebApplication.CreateBuilder(args);

builder.BootstrapExternalConfiguration();

builder.Services.AddApplicationConfiguration(builder.Configuration);

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

builder.Services.AddPostgresDatabase();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityEndpoints();

app.Run();