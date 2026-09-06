using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 032 — MeterProviderAndReader (otel-sdk).
// Goal:   Meet the metric half of the SDK, where nothing happens until somebody
//         collects.
// Drills: Sdk.CreateMeterProviderBuilder, AddMeter, the reader, manual collection.
// Passes: recording a measurement exports nothing until a collection is requested;
//         after one, the instrument arrives under its own name with the summed value;
//         a meter that was never registered exports nothing at all;
//         and measurements taken before the provider existed are simply gone.
//
// The first clause is the shape of the whole metrics pipeline and the reason metrics
// are cheap. A trace exports a span per operation; metrics aggregate in memory and hand
// over a summary when the reader asks - once a minute in production, on demand here. So
// a counter incremented a million times costs a million interlocked adds and exports
// one number.
//
// The consequence is the fourth clause, and it is the counterpart to row 027's finding
// about the tracer provider. There is no backlog: an instrument that was written to
// before a provider existed had nowhere to aggregate, so those measurements were not
// buffered, not queued, and not late - they never happened as far as any reader is
// concerned. Metrics recorded during startup, before the host is built, are lost, and
// nothing reports it.
//
// AddMeter matches by NAME, exactly like AddSource. A typo is silence.
public static class Ex032_MeterProviderAndReader
{
    /// <summary>Registered with the provider.</summary>
    public const string RegisteredMeterName = "fewolearning.telemetry.ex032";

    /// <summary>Never registered. Present so its silence can be observed.</summary>
    public const string UnregisteredMeterName = "fewolearning.telemetry.ex032.unregistered";

    /// <summary>The instrument both meters publish.</summary>
    public const string InstrumentName = "work.items";

    /// <summary>Registered with the provider.</summary>
    public static Meter Registered { get; } = new(RegisteredMeterName);

    /// <summary>Never registered.</summary>
    public static Meter Unregistered { get; } = new(UnregisteredMeterName);

    /// <summary>
    /// Build a <see cref="MeterProvider"/> that reads <see cref="RegisteredMeterName"/>
    /// and exports into <paramref name="exported"/>, collecting only when asked.
    ///
    /// The caller disposes it.
    /// </summary>
    private static readonly Counter<long> RegisteredCounter =
        Registered.CreateCounter<long>(InstrumentName);

    private static readonly Counter<long> UnregisteredCounter =
        Unregistered.CreateCounter<long>(InstrumentName);

    public static MeterProvider Build(ICollection<Metric> exported) =>
        Sdk.CreateMeterProviderBuilder()
            // By NAME, exactly like AddSource. A typo is silence.
            .AddMeter(RegisteredMeterName)
            // No periodic reader: this one collects when ForceFlush asks it to, which is
            // the whole pull model made visible.
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Add <paramref name="amount"/> to a <see cref="long"/> counter named
    /// <see cref="InstrumentName"/> on <paramref name="meter"/>.
    ///
    /// One instrument per meter, created once - not one per call.
    /// </summary>
    public static void RecordWork(Meter meter, long amount)
    {
        var counter = ReferenceEquals(meter, Registered) ? RegisteredCounter : UnregisteredCounter;

        counter.Add(amount);
    }
}
