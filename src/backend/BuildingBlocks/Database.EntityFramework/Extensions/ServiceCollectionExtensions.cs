using System.Reflection;

using Database.Abstracts;
using Database.EntityFramework.Configurations;

using Extensions;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Database.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEntityFrameworkConfiguration()
        {
            services.AddSingleton<IEntityFrameworkConfiguration, EntityFrameworkConfiguration>();
        
            return services;
        }

        public IServiceCollection AddEntityFrameworkConfiguration<TConfiguration>()
            where TConfiguration : IEntityFrameworkConfiguration
        {
            services.AddSingleton(typeof(IEntityFrameworkConfiguration), typeof(TConfiguration));
            
            return services;
        }

        public IServiceCollection AddEntityFramework<TDbContext>(params Assembly[] assembliesToScan) 
            where TDbContext : BaseDbContext, IUnitOfWork
        {
            var assembliesTypes = assembliesToScan.SelectMany(assembly => assembly.GetTypes()).ToArray();
            
            assembliesTypes
                .Where(type => type is { IsClass: true, IsAbstract: false } && 
                    type.HasInterface(typeof(ISaveChangesInterceptor)))
                .ForEach(saveChangesInterceptorType =>
                {
                    services.AddScoped(typeof(ISaveChangesInterceptor), saveChangesInterceptorType);
                });

            services.AddDbContext<TDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IEntityFrameworkConfiguration>();

                options.UseDatabase(serviceProvider, configuration.DatabaseType);

                var saveChangesInterceptors = serviceProvider.GetServices<ISaveChangesInterceptor>();
                
                options.AddInterceptors(saveChangesInterceptors);
                
                if (configuration.EnableDetailedErrors)
                {
                    options.EnableDetailedErrors();
                }

                if (configuration.EnableSensitiveDataLogging)
                {
                    options.EnableSensitiveDataLogging();
                }
            });

            assembliesTypes
                .Where(type => type.IsInterface && type.HasInterface(typeof(IRepository<,>)))
                .ForEach(repositoryInterfaceType =>
                {
                    assembliesTypes
                        .Where(type => type is { IsClass: true, IsAbstract: false } && 
                            type.HasInterface(repositoryInterfaceType))
                        .ForEach(repositoryType =>
                        {
                            services.AddScoped(repositoryInterfaceType, repositoryType);
                        });
                });

            services.AddScoped<IUnitOfWork, TDbContext>(provider => provider.GetRequiredService<TDbContext>());
            services.AddScoped<TDbContext>();
        
            return services;
        }

        public IServiceCollection AddPostgresDatabaseConfiguration()
        {
            services.AddSingleton<IPostgresDatabaseConfiguration, PostgresDatabaseConfiguration>();
            
            return services;
        }

        public IServiceCollection AddPostgresDatabaseConfiguration<TConfiguration>()
            where TConfiguration : IPostgresDatabaseConfiguration
        {
            services.AddSingleton(typeof(IPostgresDatabaseConfiguration), typeof(TConfiguration));
            
            return services;
        }
    }
}