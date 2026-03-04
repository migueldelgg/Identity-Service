namespace IdentityService.Infrastructure.Database;

// modificador sealed significa que ela não pode ser herdada por nenhuma outra classe 
public sealed class PostgresSettings
{
    private string Host { get; }
    private int Port { get; }
    private string Database { get; }
    private string Username { get; }
    private string Password { get; }

    public string ConnectionString =>
        $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";

    private PostgresSettings(string host, int port, string database, string username, string password)
    {
        Host = host;
        Port = port;
        Database = database;
        Username = username;
        Password = password;
    }

    public static PostgresSettings FromEnvironment(IConfiguration config)
    {
        // NÃO vai containerizar a API ainda, é localhost.
        // (Se no futuro containerizar, vira "postgres")
        const string host = "localhost";

        var database = Require(config["POSTGRES_DB"], "POSTGRES_DB");
        var username = Require(config["POSTGRES_USER"], "POSTGRES_USER");
        var password = Require(config["POSTGRES_PASSWORD"], "POSTGRES_PASSWORD");

        var portRaw = Require(config["POSTGRES_PORT"], "POSTGRES_PORT");
        if (!int.TryParse(portRaw, out var port))
            throw new InvalidOperationException($"Variável POSTGRES_PORT inválida: '{portRaw}' (deve ser número).");

        return new PostgresSettings(host, port, database, username, password);
    }

    private static string Require(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Variável de ambiente obrigatória não encontrada: {name}");
        return value.Trim();
    }
}