using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode;

public class GeonodeProxyServerSource : IProxyServerSource
{
    private readonly ITracer<GeonodeProxyServerSource> _tracer;
    private readonly ILogger<GeonodeProxyServerSource> _logger;
    private readonly GeonodeClient _client;
    
    public GeonodeProxyServerSource(
        ITracer<GeonodeProxyServerSource> tracer,
        ILogger<GeonodeProxyServerSource> logger,
        GeonodeClient client)
    {
        _tracer = tracer;
        _logger = logger;
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Provide proxy servers ({ProxyServerSourceConstants.Geonode})");
        
        _logger.LogInformation("Provide proxy servers Source: {ProxyServersSource}", 
            ProxyServerSourceConstants.Geonode);
        
        var currentPage = 1;
        var hasNextPage = false;

        do
        {
            var listResult = await _client.GetProxyServerListAsync(currentPage, ct);
            if (listResult.IsFailure)
            {
                yield break;
            }
            
            var pagedResult = listResult.Result!;

            hasNextPage = pagedResult.Page * pagedResult.Limit < pagedResult.Total;

            if (hasNextPage)
            {
                currentPage += 1;
            }

            yield return pagedResult.Data
                .Select(proxyServer => new ProxyServerDto
                {
                    Protocol = Enum.GetValues<ProxyServerProtocol>().First(protocol =>
                        string.Equals(protocol.ToString(), proxyServer.Protocols.ElementAt(0),
                            StringComparison.CurrentCultureIgnoreCase)),
                    HostnameOrAddress = proxyServer.Host,
                    Port = proxyServer.Port
                })
                .ToArray();
        } 
        while (hasNextPage);
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }
}