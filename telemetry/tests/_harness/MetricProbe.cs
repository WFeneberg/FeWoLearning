using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A manual-collect <see cref="MeterProvider"/> over EXACTLY ONE meter name.
/// <see cref="Collect"/> flushes and returns everything gathered since the probe was
/// created.
///
/// Call <see cref="Collect"/> TWICE when the subject is aggregation: a single
/// collection cannot tell Delta from Cumulative, cannot show a counter is monotonic,
/// and cannot catch double counting from a leaked listener.
/// </summary>
public sealed class MetricProbe : IDisposable
{
    private readonly List<Metric> _exported = [];
    private readonly MeterProvider _provider;

    public MetricProbe(string meterName)
    {
        _provider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(meterName)
            .AddInMemoryExporter(_exported)
            .Build();
    }

    public IReadOnlyList<Metric> Collect()
    {
        _provider.ForceFlush();
        return _exported.ToArray();
    }

    public void Dispose() => _provider.Dispose();
}
