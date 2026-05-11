using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using Result;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.HideMyName;

public class HideMyNameClient : IDisposable
{
    private readonly ITracer<HideMyNameClient> _tracer;
    private readonly ILogger<HideMyNameClient> _logger;
    private readonly HttpClient _httpClient;

    public HideMyNameClient(
        ITracer<HideMyNameClient> tracer,
        ILogger<HideMyNameClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _tracer = tracer;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.HideMyName);
    }
    
    public async Task<AppResult<Stream>> GetProxyListHtmlPageAsync(int offset = 0, CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list html page ({ProxyServerSourceConstants.HideMyName})");
        
        _logger.LogInformation($"Get proxy server list html page ({ProxyServerSourceConstants.HideMyName})");

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"proxy-list/?start={offset}#list", UriKind.Relative)
        };

        try
        {
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, ct);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);
                
                return AppResult<Stream>.Success(responseStream);
            }
            
            _logger.LogError("Get proxy server list html page failed StatusCode: {HttpStatusCode}", 
                httpResponseMessage.StatusCode);
        
            return AppResult<Stream>.Failure(AppErrorType.Failed, "Get proxy server list html page failed");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get proxy server list html page failed");
            
            return AppResult<Stream>.Failure(AppErrorType.Failed, "Get proxy server list html page failed");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}