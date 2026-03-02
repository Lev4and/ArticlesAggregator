using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Observability.OpenTelemetry.Configurations;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Observability.OpenTelemetry.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddOpenTelemetryConfiguration()
        {
            builder.Services.AddSingleton<IOpenTelemetryConfiguration, OpenTelemetryConfiguration>();
        
            return builder;
        }

        public WebApplicationBuilder AddOpenTelemetryConfiguration<TConfiguration>()
            where TConfiguration : class, IOpenTelemetryConfiguration
        {
            builder.Services.AddSingleton<IOpenTelemetryConfiguration, TConfiguration>();
        
            return builder;
        }

        public WebApplicationBuilder AddObservabilityOpenTelemetry(string serviceName)
        {
            builder.Host.UseSerilog((context, serviceProvider, loggerConfiguration) =>
            {
                var configuration = serviceProvider.GetRequiredService<IOpenTelemetryConfiguration>();
            
                loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Protocol = OtlpProtocol.Grpc;
                        options.Endpoint = configuration.OpenTelemetryCollectorGrpcUrl.AbsoluteUri;
                    });
            });
        
            var serviceProvider = builder.Services.BuildServiceProvider();
        
            var configuration = serviceProvider.GetRequiredService<IOpenTelemetryConfiguration>();
        
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resourceBuilder =>
                {
                    resourceBuilder.AddService(serviceName);
                })
                .WithLogging(loggerProviderBuilder =>
                {
                    loggerProviderBuilder.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = configuration.OpenTelemetryCollectorGrpcUrl;
                    });
                })
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder.AddAspNetCoreInstrumentation();
                    tracerProviderBuilder.AddEntityFrameworkCoreInstrumentation();
                    tracerProviderBuilder.AddSqlClientInstrumentation();
                    tracerProviderBuilder.AddRedisInstrumentation();
                    tracerProviderBuilder.AddHttpClientInstrumentation();
                    tracerProviderBuilder.AddGrpcClientInstrumentation();
                    tracerProviderBuilder.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = configuration.OpenTelemetryCollectorGrpcUrl;
                    });
                })
                .WithMetrics(meterProviderBuilder =>
                {
                    meterProviderBuilder.AddAspNetCoreInstrumentation();
                    meterProviderBuilder.AddEventCountersInstrumentation();
                    meterProviderBuilder.AddRuntimeInstrumentation();
                    meterProviderBuilder.AddProcessInstrumentation();
                    meterProviderBuilder.AddHttpClientInstrumentation();
                    meterProviderBuilder.AddSqlClientInstrumentation();
                    meterProviderBuilder.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.Grpc;
                        options.Endpoint = configuration.OpenTelemetryCollectorGrpcUrl;
                    });
                });
        
            return builder;
        }
    }
}