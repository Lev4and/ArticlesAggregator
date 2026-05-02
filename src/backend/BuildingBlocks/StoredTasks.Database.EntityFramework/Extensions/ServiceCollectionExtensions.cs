using System.Reflection;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoredTasks.Abstracts;

namespace StoredTasks.Database.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStoredTasks(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.HasInterface(typeof(IStoredTaskHandler<>)))
            .ForEach(storedTaskHandlerType =>
            {
                storedTaskHandlerType
                    .GetGenericInterfaces(typeof(IStoredTaskHandler<>))
                    .ForEach(handlerInterfaceType =>
                    {
                        services.TryAddScoped(handlerInterfaceType, storedTaskHandlerType);
                    });
            });
        
        return services;
    }
}