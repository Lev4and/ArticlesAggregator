using StoredTasks.Database.Abstracts;

namespace ProxyServerSearcher.Domain.Entities;

public class ProxyServerSearchPlanStoredTasks : StoredTask
{
    public DateTime PlannedAt { get; set; }
}