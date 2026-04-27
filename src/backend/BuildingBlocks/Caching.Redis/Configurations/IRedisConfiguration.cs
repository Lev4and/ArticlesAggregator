namespace Caching.Redis.Configurations;

public interface IRedisConfiguration
{
    string Host { get; }
    
    int Port { get; }
    
    string? Username { get; }
    
    string? Password { get; }
    
    string? PrefixKey { get; }
}