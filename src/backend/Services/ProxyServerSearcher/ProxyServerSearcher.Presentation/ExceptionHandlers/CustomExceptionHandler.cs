using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ProxyServerSearcher.Presentation.Constants;

namespace ProxyServerSearcher.Presentation.ExceptionHandlers;

public class CustomExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, 
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            _ => (int)HttpStatusCode.InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;
        
        var httpActivity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = "An error occurred while processing your request.",
            Status = statusCode,
            Detail = exception.Message,
            Extensions =
            {
                { ProblemDetailsExtensionConstants.RequestId, httpContext.TraceIdentifier },
                { ProblemDetailsExtensionConstants.TraceId,   httpActivity?.TraceId.ToString() },
            }
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}