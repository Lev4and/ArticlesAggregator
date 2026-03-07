using System.Net;
using Caching.Abstracts;
using Caching.Redis.Configurations;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Caching.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRedisConfiguration()
        {
            services.AddSingleton<IRedisConfiguration, RedisConfiguration>();
            
            return services;
        }

        public IServiceCollection AddRedisConfiguration<TConfiguration>()
            where TConfiguration : class, IRedisConfiguration
        {
            services.AddSingleton<IRedisConfiguration, TConfiguration>();
            
            return services;
        }
        
        public IServiceCollection AddRedisMemoryCache()
        {
            var serviceProvider = services.BuildServiceProvider();
            
            var configuration = serviceProvider.GetRequiredService<IRedisConfiguration>();
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints = [new DnsEndPoint(configuration.Host, configuration.Port)],
                    User = configuration.Username,
                    Password = configuration.Password
                };
            });

            services.AddDistributedMemoryCache();
            
            services.AddSingleton<IMemoryCache, RedisMemoryCache>();
            
            return services;
        }
    }
}