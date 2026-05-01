using Newtonsoft.Json;

namespace ProxyServerSearcher.Infrastructure.ProxyServers.Sources.Geonode.Models.Api;

public record ApiProxyServer
{
    [JsonProperty("ip")]
    public string Host { get; }
    
    [JsonProperty("port")]
    public int Port { get; }
    
    [JsonProperty("protocols")]
    public string[] Protocols { get; }

    [JsonConstructor]
    public ApiProxyServer(string host, int port, string[] protocols)
    {
        Host = host;
        Port = port;
        Protocols = protocols;
    }
}