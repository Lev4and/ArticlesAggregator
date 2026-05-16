using Messaging.Messages.ProxyServerEvents.Models;

namespace ProxyServerAggregator.Domain.Dtos.ProxyServers;

public record ProxyServerDto
{
    public Guid Id { get; init; }

    public string NormalizedName { get; init; } = null!;
    
    public ProxyServerProtocol Protocol { get; init; }
    
    public string HostnameOrAddress { get; init; } = null!;
    
    public int Port { get; init; }

    public ProxyServerCredentials? Credentials { get; init; }
}