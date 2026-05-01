using Database.EntityFramework.Configurations;
using Npgsql;

namespace Database.EntityFramework.Extensions;

public static class PostgresDatabaseConfigurationExtensions
{
    public static string GetConnectionString(this IPostgresDatabaseConfiguration configuration)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration.Host,
            Port = configuration.Port,
            Database = configuration.Database,
            Username = configuration.Username,
            Password = configuration.Password,
        };
        
        return connectionStringBuilder.ConnectionString;
    }
}