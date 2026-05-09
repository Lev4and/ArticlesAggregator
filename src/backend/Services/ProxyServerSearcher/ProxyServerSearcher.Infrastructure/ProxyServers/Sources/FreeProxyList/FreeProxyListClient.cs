using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList.Models.Api;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList;

public class FreeProxyListClient : IDisposable
{
    private readonly ITracer<FreeProxyListClient> _tracer;
    private readonly ILogger<FreeProxyListClient> _logger;
    private readonly HttpClient _httpClient;

    public FreeProxyListClient(
        ITracer<FreeProxyListClient> tracer,
        ILogger<FreeProxyListClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _tracer = tracer;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.FreeProxyList);
    }

    public async Task<ApiProxyServer[]> GetProxyServerListAsync(CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list ({ProxyServerSourceConstants.FreeProxyList})");
        
        _logger.LogInformation($"Get proxy server list ({ProxyServerSourceConstants.FreeProxyList})");
        
        var httpClientRequest = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri("proxies/all/data.json", UriKind.Relative),
        };
        
        var httpClientResponse = await _httpClient.SendAsync(httpClientRequest, ct);
        
        httpClientResponse.EnsureSuccessStatusCode();
        
        var responseJson = await httpClientResponse.Content.ReadAsStringAsync(ct);
        
        return JsonConvert.DeserializeObject<ApiProxyServer[]>(responseJson) ?? [];
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}