using Messaging.Inbox.Abstracts;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using Result;
using StoredTasks.Abstracts;

namespace Messaging.Inbox.Handling;

public class InboxMessageTaskHandler : IStoredTaskHandler<InboxMessage>
{
    private readonly ITracer<InboxMessageTaskHandler> _tracer;
    private readonly ILogger<InboxMessageTaskHandler> _logger;
    
    public InboxMessageTaskHandler(
        ITracer<InboxMessageTaskHandler> tracer, 
        ILogger<InboxMessageTaskHandler> logger)
    {
        _tracer = tracer;
        _logger = logger;
    }
    
    public Task<AppResult> HandleAsync(InboxMessage storedTask, CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation("Inbox message task handle");
        
        _logger.LogInformation("Inbox message task handle");

        return Task.FromResult(AppResult.Success());
    }
}