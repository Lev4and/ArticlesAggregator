using Messaging.Abstracts.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StoredTasks.Abstracts.Extensions;

namespace ProxyServerTester.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddStoredTaskHandlers(assembly);
        services.AddMessageHandlers(assembly);
        services.AddMediator(options =>
        {
            options.ServiceLifetime              = ServiceLifetime.Scoped;
            options.Assemblies                   = [typeof(ServiceCollectionExtensions).Assembly];
            options.Telemetry.ActivitySourceName = "Cqrs.Mediator";
            options.Telemetry.EnableMetrics      = true;
            options.Telemetry.EnableTracing      = true;
        });
        
        return services;
    }
}