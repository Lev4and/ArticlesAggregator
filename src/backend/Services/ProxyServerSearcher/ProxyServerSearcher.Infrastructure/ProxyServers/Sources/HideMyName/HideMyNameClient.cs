using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.HideMyName;

public class HideMyNameClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public HideMyNameClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.HideMyName);
    }
    
    public async Task<Stream> GetProxyListHtmlPageAsync(int offset = 0, CancellationToken ct = default)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri($"proxy-list/?start={offset}#list", UriKind.Relative)
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