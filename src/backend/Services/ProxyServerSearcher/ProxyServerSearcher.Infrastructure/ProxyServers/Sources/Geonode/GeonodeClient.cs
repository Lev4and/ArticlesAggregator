using Newtonsoft.Json;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode.Models.Api;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode;

public class GeonodeClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public GeonodeClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.Geonode);
    }

    public async Task<ApiPagedResult<ApiProxyServer>> GetProxyServerListAsync(int page = 1,
        CancellationToken cancellationToken = default)
    {
        var httpClientRequest = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"proxy-list?limit=500&page={page}&sort_by=lastChecked&sort_type=desc", 
                UriKind.Relative),
        };
        
        var httpClientResponse = await _httpClient.SendAsync(httpClientRequest, cancellationToken);
        
        httpClientResponse.EnsureSuccessStatusCode();
        
        var responseJson = await httpClientResponse.Content.ReadAsStringAsync(cancellationToken);
        
        return JsonConvert.DeserializeObject<ApiPagedResult<ApiProxyServer>>(responseJson) 
            ?? new ApiPagedResult<ApiProxyServer>([], 1, 0, 0);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}