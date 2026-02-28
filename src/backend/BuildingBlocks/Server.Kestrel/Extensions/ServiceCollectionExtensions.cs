using Microsoft.Extensions.DependencyInjection;

using Server.Kestrel.Configurations;

namespace Server.Kestrel.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddKestrelServerConfiguration()
        {
            services.AddSingleton<IKestrelServerConfiguration, KestrelServerConfiguration>();
        
            return services;
        }

        public IServiceCollection AddKestrelServerConfiguration<TConfiguration>()
            where TConfiguration : IKestrelServerConfiguration
        {
            services.AddSingleton(typeof(IKestrelServerConfiguration), typeof(TConfiguration));
            
            return services;
        }
    }
}