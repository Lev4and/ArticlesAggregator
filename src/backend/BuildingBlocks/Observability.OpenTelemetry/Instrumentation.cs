using System.Diagnostics;
using System.Diagnostics.Metrics;
using Observability.Abstracts;

namespace Observability.OpenTelemetry;

public class Instrumentation : IInstrumentation
{
    public ActivitySource ActivitySource { get; }
    
    public Meter Meter { get; }

    public Instrumentation(string name, string version = "1.0.0", Dictionary<string, object?>? tags = null)
    {
        ActivitySource = new ActivitySource(name, version, tags);
        Meter = new Meter(name, version);
    }
    
    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();
        
        GC.SuppressFinalize(this);
    }
}