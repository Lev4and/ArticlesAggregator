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
    
    public string Host => Environment.GetEnvironmentVariable(POSTGRES_HOST) 
        ?? throw new ArgumentException($"{nameof(POSTGRES_HOST)} environment variable is not set.");
    
    public int Port => int.Parse(Environment.GetEnvironmentVariable(POSTGRES_PORT) 
        ?? throw new ArgumentException($"{nameof(POSTGRES_PORT)} environment variable is not set."));
    
    public string Database => Environment.GetEnvironmentVariable(POSTGRES_DATABASE) 
        ?? throw new ArgumentException($"{nameof(POSTGRES_DATABASE)} environment variable is not set.");
    
    public string Username => Environment.GetEnvironmentVariable(POSTGRES_USERNAME) 
        ?? throw new ArgumentException($"{nameof(POSTGRES_USERNAME)} environment variable is not set.");
    
    public string Password => Environment.GetEnvironmentVariable(POSTGRES_PASSWORD) 
        ?? throw new ArgumentException($"{nameof(POSTGRES_PASSWORD)} environment variable is not set.");
}