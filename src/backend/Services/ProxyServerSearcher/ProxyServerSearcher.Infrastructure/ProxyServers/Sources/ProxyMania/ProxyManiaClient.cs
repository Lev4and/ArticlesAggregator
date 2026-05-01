using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.ProxyMania;

public class ProxyManiaClient : IDisposable
{
    private readonly HttpClient _httpClient;
    
    public ProxyManiaClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.ProxyMania);
    }

    public async Task<Stream> GetFreeProxyListHtmlPageAsync(int page = 1, CancellationToken ct = default)
    {
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