namespace ProxyServerTester.Application.Dtos.ProxyServers;

public record ProxyServerTestRequestResult
{
    public Guid Id { get; init; }
    
    public long? RequestTime { get; init; }
    
    public long? ResponseTime { get; init; }
    
    public string? ErrorMessage { get; init; }
}