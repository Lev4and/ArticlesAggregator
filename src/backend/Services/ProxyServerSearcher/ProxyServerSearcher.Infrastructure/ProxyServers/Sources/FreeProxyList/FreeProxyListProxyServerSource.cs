using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Observability.Abstracts;
using ProxyServerSearcher.Application.Abstracts.ProxyServers;
using ProxyServerSearcher.Application.Dtos.ProxyServers;
using ProxyServerSearcher.Domain.Enums;
using ProxyServerSearcher.Infrastructure.ProxyServers.Constants;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList;

public class FreeProxyListProxyServerSource : IProxyServerSource
{
    private readonly ITracer<FreeProxyListProxyServerSource> _tracer;
    private readonly ILogger<FreeProxyListProxyServerSource> _logger;
    private readonly FreeProxyListClient _client;
    
    public FreeProxyListProxyServerSource(
        ITracer<FreeProxyListProxyServerSource> tracer,
        ILogger<FreeProxyListProxyServerSource> logger,
        FreeProxyListClient client)
    {
        _tracer = tracer;
        _logger = logger;
        _client = client;
    }
    
    public async IAsyncEnumerable<IReadOnlyCollection<ProxyServerDto>> ProvideAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var operation =
            _tracer.StartOperation($"Provide proxy servers ({ProxyServerSourceConstants.FreeProxyList})");
        
        _logger.LogInformation("Provide proxy servers Source: {ProxyServersSource}", 
            ProxyServerSourceConstants.FreeProxyList);
        
        var listResult = await _client.GetProxyServerListAsync(ct);
        if (listResult.IsSuccess)
        {
            yield return listResult.Result!
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
    }

    public void Dispose()
    {
        _client.Dispose();
        
        GC.SuppressFinalize(this);
    }
}