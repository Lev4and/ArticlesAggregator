namespace Database.EntityFramework.Configurations;

public interface IPostgresDatabaseConfiguration
{
    string Host { get; }
    
    int Port { get; }
    
    string Database { get; }
    
    string Username { get; }
    
    string Password { get; }
}