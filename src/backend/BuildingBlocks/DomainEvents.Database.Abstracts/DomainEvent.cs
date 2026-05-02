using System.Text.Json;
using Contracts;
using DomainEvents.Abstracts;
using Primitives;
using StoredTasks.Database.Abstracts;

namespace DomainEvents.Database.Abstracts;

public class DomainEvent : StoredTask
{
    public string Type { get; set; } = null!;

    public JsonDocument Content { get; set; } = null!;
}