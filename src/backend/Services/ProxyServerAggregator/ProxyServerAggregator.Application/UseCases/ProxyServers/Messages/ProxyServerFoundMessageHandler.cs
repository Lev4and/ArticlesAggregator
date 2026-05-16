using Messaging.Abstracts;
using Messaging.Messages.ProxyServerEvents;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.UseCases.ProxyServers.Messages;

public class ProxyServerFoundMessageHandler : IMessageHandler<ProxyServerFoundEvent>
{
    private readonly ITracer<ProxyServerFoundMessageHandler> _tracer;
    private readonly ILogger<ProxyServerFoundMessageHandler> _logger;
    private readonly IProxyServerService _proxyServerService;
    
    public ProxyServerFoundMessageHandler(
        ITracer<ProxyServerFoundMessageHandler> tracer, 
        ILogger<ProxyServerFoundMessageHandler> logger, 
        IProxyServerService proxyServerService)
    {
        _tracer = tracer;
        _logger = logger;
        _proxyServerService = proxyServerService;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerFoundEvent message, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server found message handle");
        
        _logger.LogInformation("Proxy server found message handle Id: {MessageId}", message.Id);
        
        var proxyServerDto = new ProxyServerDto
        {
            Id                = message.ProxyServerId,
            NormalizedName    = message.NormalizedName, 
            Protocol          = message.Protocol,
            HostnameOrAddress = message.HostnameOrAddress,
            Port              = message.Port,
            Credentials       = message.Credentials
        };

        var createResult = await _proxyServerService.CreateAsync(proxyServerDto, ct);
        if (createResult.IsFailure)
        {
            var error = createResult.Errors.First();

            return error.Type is AppErrorType.Failed
                ? AppResult.Failure(createResult)
                : AppResult.Success();
        }

        return AppResult.Success();
    }
}