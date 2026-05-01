using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList;

public class FreeProxyListProxyServerSource : IProxyServerSource
{
    private readonly FreeProxyListClient _client;
    
    public FreeProxyListProxyServerSource(FreeProxyListClient client)
    {
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var proxyServerList = await _client.GetProxyServerListAsync(ct);

        yield return proxyServerList
            .Select(proxyServer => new ProxyServerDto
            {
                Protocol          = Enum.GetValues<ProxyServerProtocol>().First(protocol =>
                    string.Equals(protocol.ToString(), proxyServer.Protocol,
                        StringComparison.CurrentCultureIgnoreCase)),
                HostnameOrAddress = proxyServer.Host, 
                Port              = proxyServer.Port
            })
            .ToImmutableList();
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }
}