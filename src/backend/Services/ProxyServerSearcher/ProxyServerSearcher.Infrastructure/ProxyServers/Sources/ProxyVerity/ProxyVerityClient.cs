using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyVerity;

public class ProxyVerityClient : IDisposable
{
    private readonly HttpClient _httpClient;
    
    public ProxyVerityClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.ProxyVerity);
    }
    
    public async Task<Stream> GetFreeProxyListHtmlPageAsync(int page = 1, CancellationToken ct = default)
    {
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