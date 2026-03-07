using System.Reflection;
using DotPulsar;
using Extensions;
using Messaging.Abstracts;
using Messaging.Abstracts.Distributed;
using Messaging.Pulsar.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Messaging.Pulsar.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPulsarConfiguration()
        {
            services.AddSingleton<IPulsarConfiguration, PulsarConfiguration>();
        
            return services;
        }

        public IServiceCollection AddPulsarConfiguration<TConfiguration>()
            where TConfiguration : class, IPulsarConfiguration
        {
            services.AddSingleton<IPulsarConfiguration, TConfiguration>();
            
            return services;
        }

        public IServiceCollection AddPulsarMessaging(params Assembly[] assemblies)
        {
            var assembliesTypes = assemblies.GetTypes().ToArray();
            
            services.AddSingleton(serviceProvider =>
            {
                var pulsarConfiguration = serviceProvider.GetRequiredService<IPulsarConfiguration>();
                var pulsarClientBuilder = PulsarClient.Builder();

                pulsarClientBuilder.ServiceUrl(pulsarConfiguration.Url);

                return pulsarClientBuilder.Build();
            });

            services.AddSingleton<PulsarProducerDictionary>();

            services.AddSingleton<IDistributedMessageProducer, PulsarProducer>();
        
            assembliesTypes
                .Where(type => type is { IsClass: true, IsAbstract: false } && 
                    type.HasInterface(typeof(IMessageHandler<>)))
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
}