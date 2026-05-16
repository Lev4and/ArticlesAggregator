
using Messaging.Messages.ProxyServerEvents.Models;

namespace ProxyServerSearcher.Application.Dtos.ProxyServers;

public record ProxyServerDto
{
    public string NormalizedName => $"{Protocol.ToString().ToUpper()}-{HostnameOrAddress}-{Port}";
    
    public ProxyServerProtocol Protocol { get; init; }
    
    public string HostnameOrAddress { get; init; } = null!;
    
    public int Port { get; init; }

    public ProxyServerCredentials? Credentials { get; init; }
}