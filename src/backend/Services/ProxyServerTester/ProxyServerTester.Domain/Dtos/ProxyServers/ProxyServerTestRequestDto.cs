namespace ProxyServerTester.Domain.Dtos.ProxyServers;

public record ProxyServerTestRequestDto
{
    public Guid Id { get; init; }

    public ProxyServerDto ProxyServer { get; init; } = null!;
}