namespace Database.EntityFramework.Configurations;

public class PostgresDatabaseConfiguration : IPostgresDatabaseConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string POSTGRES_HOST = nameof(POSTGRES_HOST);
    private const string POSTGRES_PORT = nameof(POSTGRES_PORT);
    private const string POSTGRES_DATABASE = nameof(POSTGRES_DATABASE);
    private const string POSTGRES_USERNAME = nameof(POSTGRES_USERNAME);
    private const string POSTGRES_PASSWORD = nameof(POSTGRES_PASSWORD);
    
    // ReSharper restore InconsistentNaming
    
    public string Host => Environment.GetEnvironmentVariable(POSTGRES_HOST) ?? "localhost";
    
    public int Port => int.Parse(Environment.GetEnvironmentVariable(POSTGRES_PORT) ?? "5432");
    
    public string Database => Environment.GetEnvironmentVariable(POSTGRES_DATABASE) ?? "postgres";
    
    public string Username => Environment.GetEnvironmentVariable(POSTGRES_USERNAME) ?? "postgres";
    
    public string Password => Environment.GetEnvironmentVariable(POSTGRES_PASSWORD) ?? "postgres";
}