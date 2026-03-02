using System.Diagnostics.Metrics;

namespace Observability.Abstracts;

public interface IMeter
{
    Meter Meter { get; }
}