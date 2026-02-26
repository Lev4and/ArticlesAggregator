using System.Reflection;
using Extensions;
using Messaging.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Messaging.Channel.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelMessaging(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesTypes = assemblies.GetTypes().ToArray();
        
        services.AddSingleton<ChannelQueueDictionary>();
        services.AddSingleton<ChannelExchange>();
        
        services.AddSingleton<IMessageProducer, ChannelProducer>();
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                type.IsAssignableTo(typeof(ChannelConsumerWorker)))
            .ForEach(channelConsumerWorker =>
            {
                services.AddSingleton(typeof(IHostedService), channelConsumerWorker);
            });
        
        assembliesTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && type.HasInterface(typeof(IMessageHandler<>)))
            .ForEach(messageHandlerType =>
            {
                messageHandlerType
                    .GetGenericInterfaces(typeof(IMessageHandler<>))
                    .ForEach(handlerInterfaceType =>
                    {
                        services.TryAddScoped(handlerInterfaceType, messageHandlerType);
                    });
            });
        
        return services;
    }

}