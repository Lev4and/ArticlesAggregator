using System.Reflection;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Messaging.Abstracts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                type.HasInterface(typeof(IMessageHandler<>)))
            .ForEach(messageHandlerType =>
            {
                messageHandlerType.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsGenericType && 
                        interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                    .ForEach(messageHandlerInterfaceType =>
                    {
                        services.TryAddScoped(messageHandlerInterfaceType, messageHandlerType);
                    });
            });
        
        return services;
    }
}