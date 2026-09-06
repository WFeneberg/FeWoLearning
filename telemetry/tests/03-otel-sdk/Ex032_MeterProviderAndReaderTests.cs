using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex032_MeterProviderAndReaderTests
{
    /// <summary>
    /// The sum of the newest collection, snapshotted IMMEDIATELY. The in-memory exporter
    /// hands back the same Metric object on every collection, so a value read later is a
    /// later collection's value.
    /// </summary>
    private static double SumOf(IReadOnlyList<Metric> exported, string instrument) =>
        MetricReadout.Of(exported).Where(p => p.Instrument == instrument).Sum(p => p.Sum);

    [Fact]
    public void Adversarial_A_Nothing_is_exported_until_a_collection_is_requested()
    {
        // The shape of the whole metrics pipeline, and why metrics are cheap. A trace
        // exports a span per operation; metrics aggregate in memory and hand over a
        // summary when the reader asks. A counter incremented a million times costs a
        // million interlocked adds and exports one number.
        var exported = new List<Metric>();

        using var provider = Ex032_MeterProviderAndReader.Build(exported);
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Registered, 5);

        Assert.Empty(exported);
    }

    [Fact]
    public void After_a_collection_the_instrument_arrives_with_its_summed_value()
    {
        var exported = new List<Metric>();

        using var provider = Ex032_MeterProviderAndReader.Build(exported);
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Registered, 5);
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Registered, 2);
        provider.ForceFlush();

        Assert.Equal(7d, SumOf(exported, Ex032_MeterProviderAndReader.InstrumentName));
    }

    [Fact]
    public void Adversarial_B_A_meter_that_was_never_registered_exports_nothing()
    {
        // AddMeter matches by NAME, exactly like AddSource. A typo is silence.
        //
        // The same RecordWork method is used for both meters - it takes the meter as a
        // parameter - so this silence cannot come from the exercise skipping one of them.
        var exported = new List<Metric>();

        using var provider = Ex032_MeterProviderAndReader.Build(exported);
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Unregistered, 5);
        provider.ForceFlush();

        Assert.Equal(0d, SumOf(exported, Ex032_MeterProviderAndReader.InstrumentName));
    }

    [Fact]
    public void Adversarial_C_Measurements_taken_before_the_provider_existed_are_gone()
    {
        // The counterpart to row 027's finding about the tracer provider, and the sharper
        // half: there is no backlog. An instrument written to before a provider existed
        // had nowhere to aggregate, so those measurements were not buffered, not queued
        // and not late - as far as any reader is concerned they never happened.
        //
        // Metrics recorded during startup, before the host is built, are lost, and
        // nothing reports it.
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Registered, 1000);

        var exported = new List<Metric>();
        using var provider = Ex032_MeterProviderAndReader.Build(exported);
        Ex032_MeterProviderAndReader.RecordWork(Ex032_MeterProviderAndReader.Registered, 5);
        provider.ForceFlush();

        Assert.Equal(5d, SumOf(exported, Ex032_MeterProviderAndReader.InstrumentName));
    }
}
