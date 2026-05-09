using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

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
    
    public async Task<Stream> GetFreeProxyListHtmlPageAsync(int page = 1, CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyVerity})");
        
        _logger.LogInformation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyVerity})");
        
        var httpRequestMessage = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"free-proxy-list/?limit=100&proxy_page={page}", UriKind.Relative)
        };
        
        var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, ct);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        return await httpResponseMessage.Content.ReadAsStreamAsync(ct);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}