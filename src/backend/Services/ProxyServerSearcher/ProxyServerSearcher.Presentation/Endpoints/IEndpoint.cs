using Microsoft.AspNetCore.Routing;

namespace ProxyServerSearcher.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}