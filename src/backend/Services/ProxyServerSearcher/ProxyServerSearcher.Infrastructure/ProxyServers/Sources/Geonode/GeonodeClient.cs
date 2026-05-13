using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode.Models.Api;
using Result;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode;

public class GeonodeClient : IDisposable
{
    private readonly ITracer<GeonodeClient> _tracer;
    private readonly ILogger<GeonodeClient> _logger;
    private readonly HttpClient _httpClient;

    public GeonodeClient(
        ITracer<GeonodeClient> tracer,
        ILogger<GeonodeClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _tracer = tracer;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(ProxyServerSourceConstants.Geonode);
    }

    public async Task<AppResult<ApiPagedResult<ApiProxyServer>>> GetProxyServerListAsync(int page = 1,
        CancellationToken ct = default)
    {
        using var operation = _tracer.StartOperation($"Get proxy server list ({ProxyServerSourceConstants.Geonode})");
        
        _logger.LogInformation($"Get proxy server list ({ProxyServerSourceConstants.Geonode})");

        var httpClientRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"proxy-list?limit=500&page={page}&sort_by=lastChecked&sort_type=desc", 
                UriKind.Relative),
        };
        
        try
        {
            var httpResponseMessage = 
                await _httpClient.SendAsync(httpClientRequest, HttpCompletionOption.ResponseHeadersRead, ct);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var jsonSerializer = new JsonSerializer();
                
                await using var httpResponseStream = await httpResponseMessage.Content.ReadAsStreamAsync(ct);
                
                using var streamReader         = new StreamReader(httpResponseStream);
                await using var jsonTextReader = new JsonTextReader(streamReader);

                var proxyServers = jsonSerializer.Deserialize<ApiPagedResult<ApiProxyServer>>(jsonTextReader)
                    ?? new ApiPagedResult<ApiProxyServer>([], 1, 0, 0);
                
                return AppResult<ApiPagedResult<ApiProxyServer>>.Success(proxyServers);
            }

            _logger.LogError("Get proxy server list failed StatusCode: {HttpStatusCode}", 
                httpResponseMessage.StatusCode);
        
            return AppResult<ApiPagedResult<ApiProxyServer>>.Failure(AppErrorType.Failed, "Get proxy server list failed");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get proxy server list failed");
            
            return AppResult<ApiPagedResult<ApiProxyServer>>.Failure(AppErrorType.Failed, "Get proxy server list failed");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}