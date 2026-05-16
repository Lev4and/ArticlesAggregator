using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerTester.Application.Abstracts.ProxyServers;
using ProxyServerTester.Application.Dtos.ProxyServers;
using ProxyServerTester.Domain.Dtos.ProxyServers;
using ProxyServerTester.Domain.Entities;
using Result;
using StoredTasks.Abstracts;

namespace ProxyServerTester.Application.UseCases.ProxyServers.Tasks;

public class ProxyServerTestStoredTaskHandler : IStoredTaskHandler<ProxyServerTestStoredTask>
{
    private readonly ITracer<ProxyServerTestStoredTaskHandler> _tracer;
    private readonly ILogger<ProxyServerTestStoredTaskHandler> _logger;
    private readonly IProxyServerTestRequestService            _testRequestService;
    private readonly IProxyServerTester                        _proxyServerTester;
    
    public ProxyServerTestStoredTaskHandler(
        ITracer<ProxyServerTestStoredTaskHandler> tracer, 
        ILogger<ProxyServerTestStoredTaskHandler> logger,
        IProxyServerTestRequestService            testRequestService,
        IProxyServerTester                        proxyServerTester)
    {
        _tracer             = tracer;
        _logger             = logger;
        _testRequestService = testRequestService;
        _proxyServerTester  = proxyServerTester;
    }
    
    public async Task<AppResult> HandleAsync(ProxyServerTestStoredTask storedTask, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Proxy server test task handle");
        
        _logger.LogInformation("Proxy server test task handle Id: {StoredTaskId}", storedTask.Id);
        
        var testRequest = await _testRequestService.GetAsync(storedTask.RequestId, ct);
        if (testRequest.IsFailure)
        {
            var error = testRequest.Errors.First();

            return error.Type is AppErrorType.Failed
                ? AppResult.Failure(testRequest)
                : AppResult.Success();
        }

        var testResult = await _proxyServerTester.TestAsync(testRequest.Result!.ProxyServer, ct);
        
        var requestResult = new ProxyServerTestRequestResult
        {
            Id           = testRequest.Result!.Id,
            RequestTime  = testResult.Result?.RequestTime,
            ResponseTime = testResult.Result?.ResponseTime,
            ErrorMessage = testResult.Errors.FirstOrDefault()?.Message
        };

        var updateResult = await _testRequestService.UpdateAsync(requestResult, ct);
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