using Messaging.Abstracts;
using Messaging.Messages.ProxyServerEvents;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Application.Abstracts.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using Result;

namespace ProxyServerTester.Application.UseCases.ProxyServers.Messages;

public class ProxyServerTestRequestCreatedMessageHandler : IMessageHandler<ProxyServerTestRequestCreated>
{
    private readonly ITracer<ProxyServerTestRequestCreatedMessageHandler> _tracer;
    private readonly ILogger<ProxyServerTestRequestCreatedMessageHandler> _logger;
    private readonly IProxyServerTestRequestService _testRequestService;
    
    public ProxyServerTestRequestCreatedMessageHandler(
        ITracer<ProxyServerTestRequestCreatedMessageHandler> tracer, 
        ILogger<ProxyServerTestRequestCreatedMessageHandler> logger,
        IProxyServerTestRequestService testRequestService)
    {
        _tracer = tracer;
        _logger = logger;
        _testRequestService = testRequestService;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerTestRequestCreated message, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server test request created message handle");
        
        _logger.LogInformation("Proxy server test request created message handle Id: {MessageId}", message.Id);
        
        var testRequest = new ProxyServerTestRequestDto
        {
            Id          = message.Id,
            ProxyServer = new ProxyServerDto
            {
                Id                = message.ProxyServerId,
                NormalizedName    = message.NormalizedName,
                Protocol          = message.Protocol,
                HostnameOrAddress = message.HostnameOrAddress,
                Port              = message.Port,
                Credentials       = message.Credentials,
            }
        };

        var createResult = await _testRequestService.CreateAsync(testRequest, ct);
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