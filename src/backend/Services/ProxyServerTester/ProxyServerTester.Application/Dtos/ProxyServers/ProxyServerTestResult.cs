namespace ProxyServerTester.Application.Dtos.ProxyServers;

public record ProxyServerTestResult
{
    public long RequestTime { get; init; }
    
    public long ResponseTime { get; init; }
}