using System.Diagnostics.Metrics;

namespace Observability.Abstracts;

public interface IMeter : IDisposable
{
    Meter Meter { get; }
}