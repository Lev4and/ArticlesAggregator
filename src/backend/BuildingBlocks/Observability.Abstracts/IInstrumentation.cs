using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Observability.Abstracts;

public interface IInstrumentation : IDisposable
{
    ActivitySource ActivitySource { get; }
    
    Meter Meter { get; }
}