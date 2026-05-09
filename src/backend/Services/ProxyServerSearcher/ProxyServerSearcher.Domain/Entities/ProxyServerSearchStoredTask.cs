using StoredTasks.Database.Abstracts;

namespace ProxyServerSearcher.Domain.Entities;

public class ProxyServerSearchStoredTask : StoredTask
{
    public string SourceName { get; set; } = null!;
}