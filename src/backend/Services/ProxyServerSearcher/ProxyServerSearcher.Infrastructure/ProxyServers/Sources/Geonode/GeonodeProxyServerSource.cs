using System.Runtime.CompilerServices;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode;

public class GeonodeProxyServerSource : IProxyServerSource
{
    private readonly GeonodeClient _client;
    
    public GeonodeProxyServerSource(GeonodeClient client)
    {
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var currentPage = 1;
        var hasNextPage = false;

        do
        {
            var pagedResult = await _client.GetProxyServerListAsync(currentPage, ct);

            hasNextPage = pagedResult.Page * pagedResult.Limit < pagedResult.Total;

            if (hasNextPage)
            {
                currentPage += 1;
            }

            yield return pagedResult.Data
                .Select(proxyServer => new ProxyServerDto
                {
                    Protocol          = Enum.GetValues<ProxyServerProtocol>().First(protocol =>
                        string.Equals(protocol.ToString(), proxyServer.Protocols.ElementAt(0),
                            StringComparison.CurrentCultureIgnoreCase)),
                    HostnameOrAddress = proxyServer.Host,
                    Port              = proxyServer.Port
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