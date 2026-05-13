using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using Result;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyVerity;

public class ProxyVerityClient : IDisposable
{
    private readonly ITracer<ProxyVerityClient> _tracer;
    private readonly ILogger<ProxyVerityClient> _logger;
    private readonly HttpClient _httpClient;
    
    public ProxyVerityClient(
        ITracer<ProxyVerityClient> tracer,
        ILogger<ProxyVerityClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _tracer = tracer;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.ProxyVerity);
    }
    
    public async Task<AppResult<Stream>> GetFreeProxyListHtmlPageAsync(int page = 1, CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyVerity})");
        
        _logger.LogInformation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyVerity})");
        
        var httpRequestMessage = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"free-proxy-list/?limit=100&proxy_page={page}", UriKind.Relative)
        };
        
        try
        {
            var httpResponseMessage =
                await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, ct);
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