using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex034_MetricTemporalityTests
{
    /// <summary>
    /// Runs a sequence of "add this much, then collect" steps and returns what each
    /// collection reported.
    ///
    /// The snapshot after EVERY collection is the load-bearing part. The in-memory
    /// exporter hands back the same Metric object each time, so reading the values at
    /// the end would report the last collection's numbers for every step - and a test
    /// written that way agrees with itself while measuring nothing.
    /// </summary>
    private static double[] Collections(
        MetricReaderTemporalityPreference temporality, params long[] additions)
    {
        var exported = new List<Metric>();
        var reported = new List<double>();

        using var provider = Ex034_MetricTemporality.Build(exported, temporality);

        foreach (var amount in additions)
        {
            Ex034_MetricTemporality.Add(amount);
            provider.ForceFlush();

            reported.Add(
                MetricReadout.Of(exported)
                    .Where(p => p.Instrument == Ex034_MetricTemporality.InstrumentName)
                    .Sum(p => p.Sum));
        }

        return [.. reported];
    }

    [Fact]
    public void Cumulative_reports_the_running_total()
    {
        // The backend receives a monotonically rising total and computes rates itself by
        // differencing consecutive points - which survives a lost export, because the
        // next one still carries the whole history.
        var reported = Collections(MetricReaderTemporalityPreference.Cumulative, 3, 4);

        Assert.Equal([3d, 7d], reported);
    }

    [Fact]
    public void Delta_reports_what_happened_since_the_last_collection()
    {
        // The difference is computed here instead, which is cheaper to store and is what
        // a statsd-shaped backend expects - and a lost export is a hole nobody can
        // reconstruct, because that interval's data was in the message that vanished.
        var reported = Collections(MetricReaderTemporalityPreference.Delta, 3, 4);

        Assert.Equal([3d, 4d], reported);
    }

    [Fact]
    public void Adversarial_A_A_single_collection_cannot_tell_the_two_apart()
    {
        // This track's fourth lie, promoted to a fact. One collection reports 3 either
        // way - and one collection is what most tests about metrics ever do. Collect
        // twice or measure nothing.
        var cumulative = Collections(MetricReaderTemporalityPreference.Cumulative, 3);
        var delta = Collections(MetricReaderTemporalityPreference.Delta, 3);

        Assert.Equal(cumulative, delta);
        Assert.Equal([3d], cumulative);
    }

    [Fact]
    public void Adversarial_B_An_idle_interval_reports_zero_under_delta_and_the_total_under_cumulative()
    {
        // The sharpest practical consequence, and the one that catches an implementation
        // that merely subtracts the previous export: with nothing recorded in between,
        // Delta must say zero - not repeat the last value, and not go missing.
        var cumulative = Collections(MetricReaderTemporalityPreference.Cumulative, 5, 0);
        var delta = Collections(MetricReaderTemporalityPreference.Delta, 5, 0);

        Assert.Equal([5d, 5d], cumulative);
        Assert.Equal([5d, 0d], delta);
    }
}
