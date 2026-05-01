using Database.EntityFramework.Configurations;
using Database.EntityFramework.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace Database.EntityFramework.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    extension(DbContextOptionsBuilder builder)
    {
        public DbContextOptionsBuilder UseDatabase(IServiceProvider provider, DatabaseType databaseType)
        {
            return databaseType switch
            {
                DatabaseType.Postgres => builder.UsePostgresDatabase(provider),
                _ => throw new NotSupportedException($"Database type {databaseType} not supported")
            };
        }

        private DbContextOptionsBuilder UsePostgresDatabase(IServiceProvider provider)
        {
            var configuration = provider.GetRequiredService<IPostgresDatabaseConfiguration>();
            
            builder.UseNpgsql(
                configuration.GetConnectionString(),
                options =>
                {
                    
                });
            
            return builder;
        }
    }
}