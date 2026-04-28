using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using ProxyServerSearcher.Presentation.Constants;

namespace ProxyServerSearcher.Presentation.Middleware;

public class CorrelationLoggingMiddleware : IMiddleware
{
    private readonly ILogger _logger;
    
    public CorrelationLoggingMiddleware(ILogger<CorrelationLoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var httpActivity = context.Features.Get<IHttpActivityFeature>()?.Activity;

        var traceId       = httpActivity?.TraceId.ToString() 
            ?? string.Empty;
        var correlationId = context.Request.Headers[HttpHeaderConstants.RequestId].FirstOrDefault() 
            ?? Guid.NewGuid().ToString();
        
        var loggingContext = new Dictionary<string, object>
        {
            { LoggingScopeKeyConstants.TraceId,       traceId },
            { LoggingScopeKeyConstants.CorrelationId, correlationId }
        };

        context.Items["Trace-Id"]       = loggingContext[LoggingScopeKeyConstants.TraceId];
        context.Items["Correlation-Id"] = loggingContext[LoggingScopeKeyConstants.CorrelationId];

        using var scope = _logger.BeginScope(loggingContext);
        
        await next(context);
    }
}