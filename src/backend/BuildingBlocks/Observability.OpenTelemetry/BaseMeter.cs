using System.Diagnostics.Metrics;
using Observability.Abstracts;

namespace Observability.OpenTelemetry;

public class BaseMeter : IMeter
{
    public Meter Meter { get; }

    public BaseMeter(IInstrumentation instrumentation)
    {
        Meter = instrumentation.Meter;
    }

    public void Dispose()
    {
        Meter.Dispose();
        
        GC.SuppressFinalize(this);
    }
}