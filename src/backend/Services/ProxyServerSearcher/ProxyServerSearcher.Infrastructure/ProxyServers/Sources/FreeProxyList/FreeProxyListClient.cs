using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Observability.Abstracts;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;
using ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList.Models.Api;
using Result;

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

    public async Task<AppResult<ApiProxyServer[]>> GetProxyServerListAsync(CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Get proxy server list ({ProxyServerSourceConstants.FreeProxyList})");
        
        _logger.LogInformation($"Get proxy server list ({ProxyServerSourceConstants.FreeProxyList})");

        var httpClientRequest = new HttpRequestMessage
        {
            Method     = HttpMethod.Get,
            RequestUri = new Uri("proxies/all/data.json", UriKind.Relative),
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
                
                var proxyServers = jsonSerializer.Deserialize<ApiProxyServer[]>(jsonTextReader) ?? [];
                
                return AppResult<ApiProxyServer[]>.Success(proxyServers);
            }

            _logger.LogError("Get proxy server list failed StatusCode: {HttpStatusCode}", 
                httpResponseMessage.StatusCode);
        
            return AppResult<ApiProxyServer[]>.Failure(AppErrorType.Failed, "Get proxy server list failed");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Get proxy server list failed");
            
            return AppResult<ApiProxyServer[]>.Failure(AppErrorType.Failed, "Get proxy server list failed");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        
        GC.SuppressFinalize(this);
    }
}