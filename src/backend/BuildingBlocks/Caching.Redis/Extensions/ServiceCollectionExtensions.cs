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
        public IServiceCollection AddRedisMemoryCache(IRedisConfiguration? redisConfiguration = null)
        {
            redisConfiguration ??= new RedisConfiguration();
            
            services.AddSingleton(redisConfiguration);
            
            var serviceProvider = services.BuildServiceProvider();
            var configuration   = serviceProvider.GetRequiredService<IRedisConfiguration>();
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints   = [new DnsEndPoint(configuration.Host, configuration.Port)],
                    User        = configuration.Username,
                    Password    = configuration.Password,
                };
            });

            services.AddDistributedMemoryCache();
            
            services.AddScoped<IMemoryCache, RedisMemoryCache>();
            
            return services;
        }
    }
}