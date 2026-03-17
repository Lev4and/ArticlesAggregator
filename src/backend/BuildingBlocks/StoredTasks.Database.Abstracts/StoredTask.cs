using Primitives;
using StoredTasks.Abstracts;

namespace StoredTasks.Database.Abstracts;

public abstract class StoredTask : EntityBase<Guid>, IStoredTask
{
    public StoredTaskState State { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string? WorkerId { get; set; }
    
    public DateTime AttemptDeadline { get; set; }
    
    public DateTime? Deadline { get; set; }
    
    public int? AttemptsRemaining { get; set; }
    
    public int Attempts { get; set; }
}