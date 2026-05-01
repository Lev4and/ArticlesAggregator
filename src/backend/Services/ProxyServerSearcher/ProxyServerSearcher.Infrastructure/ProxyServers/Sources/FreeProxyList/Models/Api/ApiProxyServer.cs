using Newtonsoft.Json;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.FreeProxyList.Models.Api;

public record ApiProxyServer
{
    [JsonProperty("ip")]
    public string Host { get; }
    
    [JsonProperty("port")]
    public int Port { get; }
    
    [JsonProperty("protocol")]
    public string Protocol { get; }

    [JsonConstructor]
    public ApiProxyServer(string host, int port, string protocol)
    {
        Host = host;
        Port = port;
        Protocol = protocol;
    }
}