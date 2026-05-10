using System.Reflection;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace StoredTasks.Abstracts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStoredTaskHandlers(this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                type.HasInterface(typeof(IStoredTaskHandler<>)))
            .ForEach(storedTaskHandlerType =>
            {
                storedTaskHandlerType.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType && 
                        interfaceType.GetGenericTypeDefinition() == typeof(IStoredTaskHandler<>))
                    .ForEach(storedTaskHandlerInterfaceType =>
                    {
                        services.TryAddScoped(storedTaskHandlerInterfaceType, storedTaskHandlerType);
                    });
            });
        
        return services;
    }
}