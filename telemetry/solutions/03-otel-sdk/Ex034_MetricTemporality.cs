using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 034 — MetricTemporality (otel-sdk).
// Goal:   Understand what a number in an export actually means: the total so far, or
//         what happened since last time.
// Drills: MetricReaderTemporalityPreference, Cumulative vs Delta, collecting twice.
// Passes: under Cumulative, adding 3 then collecting reports 3, and adding 4 then
//                     collecting reports 7 - the running total;
//         under Delta, the same sequence reports 3 and then 4 - what happened since the
//                     previous collection;
//         a SINGLE collection reports 3 under both, and therefore proves nothing;
//         and a collection with no measurements in between reports 0 under Delta and
//                     the unchanged total under Cumulative.
//
// The third clause is this track's fourth lie, promoted to a fact. One collection cannot
// distinguish these, and neither can a test that only ever collects once - which is most
// tests anyone writes about metrics. Collect twice or measure nothing.
//
// The choice is not a preference; it decides who owns the arithmetic. Cumulative hands
// the backend a monotonically rising total and lets IT compute rates by differencing
// consecutive points, which survives a lost export - the next one still carries the
// whole history. Delta hands over the difference already computed, which is cheaper to
// store and is what a statsd-shaped backend expects - and a lost export is a hole
// nobody can reconstruct, because that interval's data was in the message that
// vanished.
//
// A test-shaped warning, and this one had to be discovered rather than read: the
// in-memory metric exporter hands back the SAME Metric object on every collection, so a
// value read after a later collection is that later collection's value. See
// MetricProbe, which snapshots at collection time for exactly this reason.
public static class Ex034_MetricTemporality
{
    /// <summary>The meter this exercise emits from.</summary>
    public const string MeterName = "fewolearning.telemetry.ex034";

    /// <summary>The counter whose meaning changes with the temporality.</summary>
    public const string InstrumentName = "work.completed";

    /// <summary>The one meter this exercise emits from.</summary>
    public static Meter Meter { get; } = new(MeterName);

    /// <summary>
    /// Build a provider reading <see cref="MeterName"/> into
    /// <paramref name="exported"/> with the given
    /// <paramref name="temporality"/> preference, collecting only when asked.
    ///
    /// The caller disposes it.
    /// </summary>
    private static readonly Counter<long> Completed = Meter.CreateCounter<long>(InstrumentName);

    public static MeterProvider Build(
        ICollection<Metric> exported, MetricReaderTemporalityPreference temporality) =>
        Sdk.CreateMeterProviderBuilder()
            .AddMeter(MeterName)
            // The preference belongs to the READER, not to the instrument or the meter:
            // the same counter means "total so far" or "since last time" depending on
            // who is collecting it.
            .AddInMemoryExporter(
                exported, options => options.TemporalityPreference = temporality)
            .Build();

    /// <summary>
    /// Add <paramref name="amount"/> to a <see cref="long"/> counter named
    /// <see cref="InstrumentName"/>. One instrument, created once.
    /// </summary>
    public static void Add(long amount) => Completed.Add(amount);
}
