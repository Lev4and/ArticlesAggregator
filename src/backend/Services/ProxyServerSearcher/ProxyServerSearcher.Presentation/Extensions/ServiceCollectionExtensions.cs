using System.Threading.RateLimiting;
using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi;
using ProxyServerSearcher.Presentation.Constants;
using ProxyServerSearcher.Presentation.Endpoints;
using ProxyServerSearcher.Presentation.ExceptionHandlers;
using ProxyServerSearcher.Presentation.Middleware;
using Server.Kestrel.Configurations;

namespace ProxyServerSearcher.Presentation.Extensions;

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
            .AddNpgSql();
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
                        Title       = "ProxyServerSearcher",
                        Description = "Proxy Server Searcher API",
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
                    filePath: AppContext.BaseDirectory + "/ProxyServerSearcher.Domain.xml", 
                    includeControllerXmlComments: true);
                
                options.IncludeXmlComments(
                    filePath: AppContext.BaseDirectory + "/ProxyServerSearcher.Application.xml", 
                    includeControllerXmlComments: true);
                
                options.IncludeXmlComments(
                    filePath: AppContext.BaseDirectory + "/ProxyServerSearcher.Presentation.xml", 
                    includeControllerXmlComments: true);
            })
            .AddSwaggerGenNewtonsoftSupport();
        services.AddGrpc(options =>
        {
            
        });
        
        return services;
    }
}