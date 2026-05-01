using Database.Abstracts;

namespace StoredTasks.Database.Abstracts;

public interface IStoredTaskRepository<TStoredTask> : IRepository<TStoredTask, Guid>
    where TStoredTask : StoredTask
{
    
}