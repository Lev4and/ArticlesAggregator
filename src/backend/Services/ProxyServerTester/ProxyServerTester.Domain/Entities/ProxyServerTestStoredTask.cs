using StoredTasks.Database.Abstracts;

namespace ProxyServerTester.Domain.Entities;

public class ProxyServerTestStoredTask : StoredTask
{
    public Guid RequestId { get; init; }
    
    public ProxyServerTestRequest? Request { get; init; }
}