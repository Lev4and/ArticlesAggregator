using Mediator;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.UseCases.ProxyServers.Queries.Get;

public class GetProxyServerQueryHandler : IQueryHandler<GetProxyServerQuery, AppResult<ProxyServerDto>>
{
    private readonly ITracer<GetProxyServerQueryHandler> _tracer;
    private readonly ILogger<GetProxyServerQueryHandler> _logger;
    private readonly IProxyServerService _proxyServerService;
    
    public GetProxyServerQueryHandler(
        ITracer<GetProxyServerQueryHandler> tracer, 
        ILogger<GetProxyServerQueryHandler> logger, 
        IProxyServerService proxyServerService)
    {
        _tracer = tracer;
        _logger = logger;
        _proxyServerService = proxyServerService;
    }
    
    public async ValueTask<AppResult<ProxyServerDto>> Handle(GetProxyServerQuery query, 
        CancellationToken cancellationToken)
    {
        using var operation = _tracer.StartOperation("Get proxy server query handle");
        
        _logger.LogInformation("Get proxy server query handle");
        
        return await _proxyServerService.GetAsync(query.Id, cancellationToken);
    }
}