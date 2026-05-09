using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyMania;

public class ProxyManiaClient : IDisposable
{
    private readonly ITracer<ProxyManiaClient> _tracer;
    private readonly ILogger<ProxyManiaClient> _logger;
    private readonly HttpClient _httpClient;
    
    public ProxyManiaClient(
        ITracer<ProxyManiaClient> tracer,
        ILogger<ProxyManiaClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _tracer = tracer;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.ProxyMania);
    }

    public async Task<Stream> GetFreeProxyListHtmlPageAsync(int page = 1, CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyMania})");
        
        _logger.LogInformation($"Get proxy server list html page ({ProxyServerSourceConstants.ProxyMania})");
        
        var httpRequestMessage = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"free-proxy?page={page}", UriKind.Relative)
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