using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ProxyServerTester.Presentation.Endpoints;
using ProxyServerTester.Presentation.Grpc.Services;

namespace ProxyServerTester.Presentation.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder MapEndpoints(this WebApplication app, 
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpointRouteBuilder = routeGroupBuilder is not null 
            ? (IEndpointRouteBuilder)routeGroupBuilder
            : app;
        
        app.Services.GetRequiredService<IEnumerable<IEndpoint>>()
            .ForEach(endpoint => { endpoint.MapEndpoint(endpointRouteBuilder); });

        return app;
    }

    public static IApplicationBuilder MapGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<ProxyServerTesterGrpcService>();
        
        return app;
    }
}