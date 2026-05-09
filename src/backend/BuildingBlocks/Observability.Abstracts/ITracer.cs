using System.Diagnostics;

namespace Observability.Abstracts;

public interface ITracer<T> : IDisposable
{
    Activity StartOperation(string name, Dictionary<string, object?>? tags = null, 
        ActivityKind kind = ActivityKind.Internal);
}