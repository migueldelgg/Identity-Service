using System.Text;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure.Database;
using IdentityService.Infrastructure.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityService.Modules.Identity;
using IdentityService.Modules.Identity.UseCases.PasswordHashing;

// -------------------------------------------------------- //

var builder = WebApplication.CreateBuilder(args);

// 1) Carrega .env da raiz (um nível acima), as variaveis ficam disponiveiss em builder.Configuration
Env.Load(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
builder.Configuration.AddEnvironmentVariables();

var secrets = SecretsSettings.FromEnvironment(builder.Configuration);
builder.Services.AddSingleton(secrets);
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,

            ValidIssuer = secrets.JwtIssuer,
            ValidAudience = secrets.JwtAudience,
            ClockSkew = TimeSpan.Zero,

            // RS256: valida assinatura com a PUBLIC KEY
            IssuerSigningKey = TokenProvider.BuildRsaKey(secrets.JwtPublicKeyPem),
        };
    });


// 2) Cria settings validado e registra
var pg = PostgresSettings.FromEnvironment(builder.Configuration);
builder.Services.AddDbContext<DatabaseContext>(
    dbContextOptionsBuilder =>
    {
        dbContextOptionsBuilder.UseNpgsql(pg.ConnectionString, npgsqlAction =>
        {
            npgsqlAction.EnableRetryOnFailure(3);

            npgsqlAction.CommandTimeout(30);
        });

        dbContextOptionsBuilder.EnableDetailedErrors();
        dbContextOptionsBuilder.EnableSensitiveDataLogging();
    });

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityEndpoints();

app.Run();