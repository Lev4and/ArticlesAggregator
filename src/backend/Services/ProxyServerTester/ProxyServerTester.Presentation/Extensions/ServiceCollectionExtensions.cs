using System.Threading.RateLimiting;
using Database.EntityFramework.Configurations;
using Database.EntityFramework.Extensions;
using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi;
using ProxyServerTester.Presentation.Constants;
using ProxyServerTester.Presentation.Endpoints;
using ProxyServerTester.Presentation.ExceptionHandlers;
using ProxyServerTester.Presentation.Middleware;
using Server.Kestrel.Configurations;

namespace ProxyServerTester.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        
        var serviceDescriptors = typeof(IEndpoint).Assembly.GetTypes()
            .Where(type => 
                type is { IsClass: true, IsAbstract: false } && type.HasInterface(typeof(IEndpoint)))
            .Select(endpointType => ServiceDescriptor.Transient(typeof(IEndpoint), endpointType))
            .ToArray();
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                
            });
        services.TryAddEnumerable(serviceDescriptors);
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var httpActivity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

                context.ProblemDetails.Extensions.TryAdd(
                    key: ProblemDetailsExtensionConstants.RequestId,
                    value: context.HttpContext.TraceIdentifier);
                
                context.ProblemDetails.Extensions.TryAdd(
                    key: ProblemDetailsExtensionConstants.TraceId, 
                    value: httpActivity?.TraceId.ToString());
            };
        });
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            options.AddPolicy(RateLimiterPolicyNameConstants.FixedByIp, httpContext =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Request.Headers[HttpHeaderConstants.ForwardedFor].ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10, Window = TimeSpan.FromMinutes(1)
                    });
            });
        });
        services.AddHttpContextAccessor();
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = 
                HttpLoggingFields.RequestPropertiesAndHeaders | 
                HttpLoggingFields.ResponsePropertiesAndHeaders;
            
            logging.RequestHeaders.Add(HttpHeaderConstants.RequestId);
        });
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyMethod()
                        .AllowAnyHeader()
                            .SetIsOriginAllowed(_ => true)
                                .AllowCredentials();
            });
        });
        services.AddTransient<CorrelationLoggingMiddleware>();
        services
            .AddHealthChecks()
            .AddNpgSql(
                connectionStringFactory: provider =>
                    provider.GetRequiredService<IPostgresDatabaseConfiguration>().GetConnectionString(),
                tags: ["ready", "alive"]);
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services
            .AddSwaggerGen(options =>
            {
                var configuration = serviceProvider.GetService<IKestrelServerConfiguration>() 
                    ?? new KestrelServerConfiguration();
                
                options.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo
                    {
                        Title       = "ProxyServerTester",
                        Description = "Proxy Server Tester API",
                        Version     = "1.0.0"
                    });

                options.AddServer(
                    new OpenApiServer
                    {
                        Url         = "{SCHEME}://localhost:{PORT}/",
                        Description = "Local server",
                        Variables   = new Dictionary<string, OpenApiServerVariable>
                        {
                            {   
                                "SCHEME", 
                                new OpenApiServerVariable
                                {
                                    Default = "http"
                                }
                            },
                            {
                                "PORT",
                                new OpenApiServerVariable
                                {
                                    Default = configuration.HttpPort.ToString()
                                }
                            }
                        }
                    });
                
                options.IncludeXmlComments(
                    filePath: AppContext.BaseDirectory + "/ProxyServerTester.Domain.xml", 
                    includeControllerXmlComments: true);
                
                options.IncludeXmlComments(
                    filePath: AppContext.BaseDirectory + "/ProxyServerTester.Application.xml", 
                    includeControllerXmlComments: true);
            
                options.IncludeXmlComments(
                    filePath: AppContext.BaseDirectory + "/ProxyServerTester.Presentation.xml", 
                    includeControllerXmlComments: true);
            })
            .AddSwaggerGenNewtonsoftSupport();
        services.AddGrpc(options =>
        {
            
        });
        
        return services;
    }
}