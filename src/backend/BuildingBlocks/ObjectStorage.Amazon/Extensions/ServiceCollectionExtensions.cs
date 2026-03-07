using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using ObjectStorage.Abstracts;
using ObjectStorage.Amazon.Configurations;

namespace ObjectStorage.Amazon.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAmazonObjectStorageConfiguration()
        {
            services.AddSingleton<IAmazonObjectStorageConfiguration, AmazonObjectStorageConfiguration>();
            
            return services;
        }

        public IServiceCollection AddAmazonObjectStorageConfiguration<TConfiguration>()
            where TConfiguration : class, IAmazonObjectStorageConfiguration
        {
            services.AddSingleton<IAmazonObjectStorageConfiguration, TConfiguration>();
            
            return services;
        }
        
        public IServiceCollection AddAmazonObjectStorage()
        {
            services.AddSingleton<IAmazonS3>(provider =>
            {
                var configuration = provider.GetRequiredService<IAmazonObjectStorageConfiguration>();
                
                var s3Config = new AmazonS3Config
                {
                    
                };
                
                return new AmazonS3Client(s3Config);
            });
            
            services.AddSingleton<IObjectStorage, AmazonObjectStorage>();
            
            return services;
        }
    }
}