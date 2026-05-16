using Messaging.Abstracts.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StoredTasks.Abstracts.Extensions;

namespace ProxyServerAggregator.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddStoredTaskHandlers(assembly);
        services.AddMessageHandlers(assembly);
        
        return services;
    }
}