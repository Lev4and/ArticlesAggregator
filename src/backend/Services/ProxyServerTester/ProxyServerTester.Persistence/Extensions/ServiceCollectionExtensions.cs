using Database.EntityFramework.Extensions;
using Messaging.Outbox.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StoredTasks.Database.EntityFramework.Extensions;

namespace ProxyServerTester.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(AppDbContext).Assembly
        };

        services.AddPostgresDatabaseConfiguration();
        
        services.AddEntityFrameworkConfiguration();
        services.AddEntityFramework<AppDbContext>(assemblies);
        services.AddEntityFrameworkStoredTasks(assemblies);
        services.AddEntityFrameworkOutboxMessageRepository<AppDbContext>();
        
        return services;
    }
}