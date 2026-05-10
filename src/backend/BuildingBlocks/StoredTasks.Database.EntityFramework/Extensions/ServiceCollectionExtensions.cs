using System.Reflection;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoredTasks.Abstracts.Extensions;
using StoredTasks.Database.Abstracts;

namespace StoredTasks.Database.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkStoredTasks(this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();

        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                type.HasInterface(typeof(IStoredTaskRepository<>)))
            .ForEach(repositoryType =>
            {
                repositoryType.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType && 
                        interfaceType.GetGenericTypeDefinition() == typeof(IStoredTaskRepository<>))
                    .ForEach(repositoryInterfaceType =>
                    {
                        services.TryAddScoped(repositoryInterfaceType, repositoryType);
                    });
            });
        
        services.AddStoredTaskHandlers(assemblies);

        return services;
    }
}