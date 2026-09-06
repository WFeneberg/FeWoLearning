using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// A manual-collect <see cref="MeterProvider"/> over EXACTLY ONE meter name, for rows
/// whose subject is the instrument rather than the pipeline.
///
/// Rows that build their own provider - because building it IS the exercise - use
/// <see cref="MetricReadout.Of"/> directly against their own exported list instead.
///
/// <see cref="Collect"/> snapshots, and that is mandatory rather than tidy: see
/// <see cref="MetricReadout"/> for why holding on to a <see cref="Metric"/> gives an
/// answer that changes underneath you.
///
/// Collect twice whenever the subject is aggregation. One collection cannot tell Delta
/// from Cumulative, cannot show a counter is monotonic, and cannot catch double counting
/// from a leaked listener.
/// </summary>
public sealed class MetricProbe : IDisposable
{
    private readonly List<Metric> _exported = [];
    private readonly MeterProvider _provider;

    public MetricProbe(string meterName, MetricReaderTemporalityPreference? temporality = null)
    {
        var builder = Sdk.CreateMeterProviderBuilder().AddMeter(meterName);

        _provider = temporality is null
            ? builder.AddInMemoryExporter(_exported).Build()
            : builder.AddInMemoryExporter(
                _exported, options => options.TemporalityPreference = temporality.Value).Build();
    }

    /// <summary>Flush, then copy out every metric point of the newest collection.</summary>
    public IReadOnlyList<MetricPointSnapshot> Collect()
    {
        _provider.ForceFlush();

        return MetricReadout.Of(_exported);
    }

    /// <summary>The points of one instrument, by its exported name.</summary>
    public IReadOnlyList<MetricPointSnapshot> CollectFor(string instrument) =>
        Collect().Where(p => p.Instrument == instrument).ToArray();

    public void Dispose() => _provider.Dispose();
}
