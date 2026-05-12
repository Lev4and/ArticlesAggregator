using System.Text.Json;
using StoredTasks.Database.Abstracts;

namespace Messaging.Outbox.Abstracts;

public class OutboxMessage : StoredTask, IDisposable
{
    public string Type { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;

    public void Dispose()
    {
        Content.Dispose();

        GC.SuppressFinalize(this);
    }
}