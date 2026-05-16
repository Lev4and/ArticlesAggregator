using Messaging.Abstracts;
using Messaging.Messages.ProxyServerEvents;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerAggregator.Application.Abstracts.ProxyServers;
using ProxyServerAggregator.Application.Dtos.ProxyServers;
using Result;

namespace ProxyServerAggregator.Application.UseCases.ProxyServers.Messages;

public class ProxyServerTestRequestStatusChangedMessageHandler : IMessageHandler<ProxyServerTestRequestStatusChanged>
{
    private readonly ITracer<ProxyServerTestRequestStatusChangedMessageHandler> _tracer;
    private readonly ILogger<ProxyServerTestRequestStatusChangedMessageHandler> _logger;
    private readonly IProxyServerTestRequestService _testRequestService;
    
    public ProxyServerTestRequestStatusChangedMessageHandler(
        ITracer<ProxyServerTestRequestStatusChangedMessageHandler> tracer, 
        ILogger<ProxyServerTestRequestStatusChangedMessageHandler> logger, 
        IProxyServerTestRequestService testRequestService)
    {
        _tracer = tracer;
        _logger = logger;
        _testRequestService = testRequestService;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerTestRequestStatusChanged message, 
        CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server test request status changed message handle");
        
        _logger.LogInformation("Proxy server test request status changed message handle Id: {MessageId}", message.Id);

        var testResult = new ProxyServerTestRequestResult
        {
            RequestId    = message.RequestId,
            RequestTime  = message.RequestTime,
            ResponseTime = message.ResponseTime,
            ErrorMessage = message.ErrorMessage,
        };
        
        var updateResult = await _testRequestService.UpdateAsync(testResult, ct);
        if (updateResult.IsFailure)
        {
            var error = updateResult.Errors.First();

            return error.Type is AppErrorType.Failed
                ? AppResult.Failure(updateResult)
                : AppResult.Success();
        }

        return AppResult.Success();
    }
}