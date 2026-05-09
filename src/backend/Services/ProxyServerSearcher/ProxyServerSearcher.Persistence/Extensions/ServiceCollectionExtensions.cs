using Database.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StoredTasks.Database.EntityFramework.Extensions;

namespace ProxyServerSearcher.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddPostgresDatabaseConfiguration();
        
        services.AddEntityFrameworkConfiguration();
        services.AddEntityFramework<AppDbContext>(typeof(ServiceCollectionExtensions).Assembly);
        services.AddEntityFrameworkStoredTasks(typeof(ServiceCollectionExtensions).Assembly);
        
        return services;
    }
}