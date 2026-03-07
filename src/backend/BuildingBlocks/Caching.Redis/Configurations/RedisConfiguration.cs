namespace Caching.Redis.Configurations;

public class RedisConfiguration : IRedisConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string REDIS_HOST = nameof(REDIS_HOST);
    private const string REDIS_PORT = nameof(REDIS_PORT);
    private const string REDIS_USERNAME = nameof(REDIS_USERNAME);
    private const string REDIS_PASSWORD = nameof(REDIS_PASSWORD);
    
    // ReSharper restore InconsistentNaming
    
    public string Host => Environment.GetEnvironmentVariable(REDIS_HOST) 
        ?? throw new ArgumentException($"{nameof(REDIS_HOST)} environment variable is not set.");
    
    public int Port => int.Parse(Environment.GetEnvironmentVariable(REDIS_PORT) 
        ?? throw new ArgumentException($"{nameof(REDIS_PORT)} environment variable is not set."));

    public string Username => Environment.GetEnvironmentVariable(REDIS_USERNAME) 
        ?? throw new ArgumentException($"{nameof(REDIS_USERNAME)} environment variable is not set.");
    
    public string Password => Environment.GetEnvironmentVariable(REDIS_PASSWORD) 
        ?? throw new ArgumentException($"{nameof(REDIS_PASSWORD)} environment variable is not set.");
}