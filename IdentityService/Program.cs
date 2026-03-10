using Azure.Identity;
using DotNetEnv;
using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure.Database;
using IdentityService.Infrastructure.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityService.Modules.Identity;
using IdentityService.Modules.Identity.UseCases.PasswordHashing;

// -------------------------------------------------------- //

// entender como encapsular:
// ambinete de dev, conectao com new DefaultAzureCredential
// ambiente de prod se conecta como no azure key vault?
// como podemos encapsular a busca das connection strings, por exemplo:
// caso eu esteja tentando buscar uma connection string que esta em branco ou nao encontrada?
// isso explode minha aplicacao
// monitoramento disso, como fica? enviar email em caso de erro?
// incluir no cofre de senhas os certificados digitais, chave publica e privada.
// recuperar na inicializacao do programa e reutilizar para assinar e ler tokens jwt.

var builder = WebApplication.CreateBuilder(args);

// 1) Carrega .env da raiz (um nível acima), as variaveis ficam disponiveiss em builder.Configuration
Env.Load(Path.Combine(builder.Environment.ContentRootPath, "..", ".env"));
builder.Configuration.AddEnvironmentVariables();

// TODO: Encapsular bootstrap de secrets (Key Vault) com comportamento por ambiente:
// - DEV: DefaultAzureCredential resolve via Azure CLI (az login)
// - PROD (Azure): DefaultAzureCredential resolve via Managed Identity (sem mudar código)
var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
if (!string.IsNullOrWhiteSpace(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential()
    );
}
// valida se todos os secrets necessários existem
KeyVaultValidation.LoadAndRegister(builder);

// TODO: Ajustar para usar azure key vault
// também criar métodos estáticos para carregar para fazer todos estes add singleton

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
builder.Services.AddDbContext<DatabaseContext>((serviceProvider, dbContextOptionsBuilder) =>
{
    var psql = serviceProvider.GetRequiredService<AppSecrets>().PostgreSqlConnection;
    dbContextOptionsBuilder.UseNpgsql(
        psql,
        npgsqlAction =>
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