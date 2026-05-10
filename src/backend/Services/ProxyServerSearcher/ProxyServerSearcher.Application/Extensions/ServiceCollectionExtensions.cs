using Microsoft.Extensions.DependencyInjection;
using StoredTasks.Abstracts.Extensions;

namespace ProxyServerSearcher.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddStoredTaskHandlers(assembly);
        
        return services;
    }
}