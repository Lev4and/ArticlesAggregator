using Microsoft.AspNetCore.Routing;

namespace ProxyServerTester.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}