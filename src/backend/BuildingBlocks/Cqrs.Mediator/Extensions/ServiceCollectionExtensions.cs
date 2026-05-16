using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.Mediator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCqrsMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime              = ServiceLifetime.Scoped;
            options.Assemblies                   = [..assemblies];
            options.Telemetry.ActivitySourceName = "Cqrs.Mediator";
            options.Telemetry.EnableMetrics      = true;
            options.Telemetry.EnableTracing      = true;
        });
        
        return services;
    }
}