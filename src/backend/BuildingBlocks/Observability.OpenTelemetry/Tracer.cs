using System.Diagnostics;
using System.Reflection;
using Observability.Abstracts;

namespace Observability.OpenTelemetry;

public class Tracer : ITracer
{
    public Activity StartOperation(string name, Dictionary<string, object?>? tags = null, 
        ActivityKind kind = ActivityKind.Internal)
    {
        throw new NotImplementedException();
    }
}