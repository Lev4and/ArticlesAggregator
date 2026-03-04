using System.Diagnostics;

namespace Observability.Abstracts;

public interface ITracer
{
    Activity StartOperation(string name, Dictionary<string, object?>? tags = null, 
        ActivityKind kind = ActivityKind.Internal);
}