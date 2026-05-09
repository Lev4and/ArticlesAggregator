using StoredTasks.Database.Abstracts;

namespace ProxyServerSearcher.Domain.Entities;

public class ProxyServerSearchPlanStoredTask : StoredTask
{
    public DateTime PlannedAt { get; set; }
}