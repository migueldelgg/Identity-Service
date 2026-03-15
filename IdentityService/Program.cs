using FluentValidation;
using IdentityService.Infrastructure.Authentication;
using IdentityService.Infrastructure.Configuration.Extensions;
using IdentityService.Infrastructure.Database;
using IdentityService.Infrastructure.Exceptions;
using IdentityService.Modules.Identity;
using IdentityService.Modules.Identity.UseCases.PasswordHashing;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionConfiguration();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

builder.BootstrapExternalConfiguration();

builder.Services.AddApplicationConfiguration(builder.Configuration);

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

builder.Services.AddPostgresDatabase();

builder.Services.AddOpenApi();

var app = builder.Build();

// Documentation
app.MapOpenApi();
app.MapScalarApiReference("/docs");

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapIdentityEndpoints();

// Excepiton
app.UseExceptionHandler();

app.Run();