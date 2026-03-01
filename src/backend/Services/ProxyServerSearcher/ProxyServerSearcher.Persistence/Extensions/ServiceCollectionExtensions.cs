using Database.EntityFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ProxyServerSearcher.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddPostgresDatabaseConfiguration();
        
        services.AddEntityFrameworkConfiguration();
        services.AddEntityFramework<AppDbContext>();

        return services;
    }
}