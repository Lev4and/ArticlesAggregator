using System.Diagnostics;
using Observability.Abstracts;

namespace Observability.OpenTelemetry;

public class Tracer<T> : ITracer<T>
{
    private readonly IInstrumentation _instrumentation;

    public Tracer(IInstrumentation instrumentation)
    {
        _instrumentation = instrumentation;
    }
    
    public Activity StartOperation(string name, Dictionary<string, object?>? tags = null, 
        ActivityKind kind = ActivityKind.Internal)
    {
        return _instrumentation.ActivitySource.StartActivity(name: name, kind: kind, tags: tags?.ToList())!;
    }

    public void Dispose()
    {
        _instrumentation.Dispose();
        
        GC.SuppressFinalize(this);
    }
}