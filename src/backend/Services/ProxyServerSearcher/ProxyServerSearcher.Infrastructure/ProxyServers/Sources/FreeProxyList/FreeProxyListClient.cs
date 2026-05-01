using Newtonsoft.Json;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList.Models.Api;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList;

public class FreeProxyListClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public FreeProxyListClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.FreeProxyList);
    }

    public async Task<ApiProxyServer[]> GetProxyServerListAsync(CancellationToken ct = default)
    {
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