using System.Text.Json;
using StoredTasks.Database.Abstracts;

namespace Messaging.Outbox.Abstracts;

public class OutboxMessage : StoredTask
{
    public string Type { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;
}