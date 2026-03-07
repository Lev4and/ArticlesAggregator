namespace StoredTasks.Abstracts;

public interface IStoredTask
{
    Guid Id { get; set; }
    
    StoredTaskState State { get; set; }

    DateTime CreatedAt { get; set; }
    
    string? WorkerId { get; set; }

    DateTime AttemptDeadline { get; set; }

    DateTime? Deadline { get; set; }

    int? AttemptsRemaining { get; set; }
    
    int Attempts { get; set; }
}