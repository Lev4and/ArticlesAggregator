namespace Caching.Redis.Configurations;

public class RedisConfiguration : IRedisConfiguration
{
    // ReSharper disable InconsistentNaming
    
    private const string REDIS_HOST = nameof(REDIS_HOST);
    private const string REDIS_PORT = nameof(REDIS_PORT);
    private const string REDIS_USERNAME = nameof(REDIS_USERNAME);
    private const string REDIS_PASSWORD = nameof(REDIS_PASSWORD);
    private const string REDIS_PREFIX_KEY = nameof(REDIS_PREFIX_KEY);
    
    // ReSharper restore InconsistentNaming
    
    public string Host => Environment.GetEnvironmentVariable(REDIS_HOST) ?? "localhost";
    
    public int Port => int.Parse(Environment.GetEnvironmentVariable(REDIS_PORT) ?? "6379");

    public string? Username => Environment.GetEnvironmentVariable(REDIS_USERNAME);
    
    public string? Password => Environment.GetEnvironmentVariable(REDIS_PASSWORD);
    
    public string? PrefixKey => Environment.GetEnvironmentVariable(REDIS_PREFIX_KEY);
}