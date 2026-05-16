using Microsoft.AspNetCore.Routing;

namespace ProxyServerAggregator.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}