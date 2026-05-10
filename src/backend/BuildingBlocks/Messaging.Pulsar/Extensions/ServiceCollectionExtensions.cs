using System.Reflection;
using DotPulsar;
using DotPulsar.Internal;
using Messaging.Abstracts.Distributed;
using Messaging.Abstracts.Extensions;
using Messaging.Pulsar.Configurations;
using Microsoft.Extensions.DependencyInjection;

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
        
        public IServiceCollection AddPulsarTopicConfiguration()
        {
            services.AddSingleton<IPulsarTopicConfiguration, PulsarTopicConfiguration>();
        
            return services;
        }

        public IServiceCollection AddPulsarTopicConfiguration<TConfiguration>()
            where TConfiguration : class, IPulsarTopicConfiguration
        {
            services.AddSingleton<IPulsarTopicConfiguration, TConfiguration>();
            
            return services;
        }

        public IServiceCollection AddPulsarMessaging(params Assembly[] assemblies)
        {
            services.AddScoped(serviceProvider =>
            {
                var pulsarConfiguration = serviceProvider.GetRequiredService<IPulsarConfiguration>();
                var pulsarClientBuilder = PulsarClient.Builder();

                pulsarClientBuilder.ServiceUrl(pulsarConfiguration.Url);

                if (pulsarConfiguration.UseTls)
                {
                    pulsarClientBuilder.ConnectionSecurity(EncryptionPolicy.PreferEncrypted);
                }

                if (!string.IsNullOrEmpty(pulsarConfiguration.AuthToken))
                {
                    pulsarClientBuilder.Authentication(new TokenAuthentication(pulsarConfiguration.AuthToken));
                }

                return pulsarClientBuilder.Build();
            });

            services.AddSingleton<PulsarProducerDictionary>();

            services.AddScoped<IDistributedMessageProducer, PulsarProducer>();
        
            services.AddMessageHandlers(assemblies);
        
            return services;
        }
        
        public IServiceCollection AddPulsarConsumer(IPulsarConsumerConfiguration configuration)
        {
            var consumerConfigurationKey = $"PulsarConsumerConfiguration-{Guid.NewGuid().ToString()}";
            var consumerWorkerKey        = $"PulsarConsumerWorker-{Guid.NewGuid().ToString()}";
            
            services.AddKeyedSingleton(consumerConfigurationKey, configuration);

            services.AddKeyedScoped(consumerWorkerKey,
                (sp, key) => 
                    ActivatorUtilities.CreateInstance<PulsarConsumerWorker>(
                        sp,
                        sp.GetRequiredKeyedService<IPulsarConsumerConfiguration>(consumerConfigurationKey)));
        
            services.AddHostedService<PulsarConsumerWorkerService>(
                sp => 
                    ActivatorUtilities.CreateInstance<PulsarConsumerWorkerService>(
                        sp, 
                        consumerWorkerKey));
        
            return services;
        }
    }
}