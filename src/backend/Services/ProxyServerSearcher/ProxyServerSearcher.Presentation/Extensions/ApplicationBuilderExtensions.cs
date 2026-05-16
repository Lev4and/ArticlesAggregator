using Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ProxyServerSearcher.Presentation.Endpoints;
using ProxyServerSearcher.Presentation.Grpc.Services;

namespace ProxyServerSearcher.Presentation.Extensions;

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
        app.MapGrpcService<ProxyServerSearcherGrpcService>();
        
        return app;
    }
}