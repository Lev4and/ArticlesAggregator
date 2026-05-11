using System.Text.Json;
using StoredTasks.Database.Abstracts;

namespace Messaging.Inbox.Abstracts;

public class InboxMessage : StoredTask
{
    public string Type { get; set; } = null!;
    
    public JsonDocument Content { get; set; } = null!;
}