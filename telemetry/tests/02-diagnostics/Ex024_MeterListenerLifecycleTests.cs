using System.Diagnostics.Metrics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex024_MeterListenerLifecycleTests
{
    /// <summary>
    /// A meter name nobody else uses. Each test gets its own, because a Meter's
    /// instruments cannot be removed and a shared name would leak published
    /// instruments from one test into the next.
    /// </summary>
    private static string FreshMeterName() => $"fewolearning.telemetry.ex024.{Guid.NewGuid():n}";

    private sealed class Collected
    {
        private readonly List<(string Instrument, long Value)> _items = [];

        public void Add(string instrument, long value)
        {
            lock (_items) _items.Add((instrument, value));
        }

        public IReadOnlyList<(string Instrument, long Value)> Items
        {
            get { lock (_items) return _items.ToArray(); }
        }
    }

    [Fact]
    public void Measurements_from_the_named_meter_arrive()
    {
        var name = FreshMeterName();
        var collected = new Collected();
        using var meter = new Meter(name);
        var counter = meter.CreateCounter<long>("things");

        using var listener = Ex024_MeterListenerLifecycle.CreateListener(name, collected.Add);
        counter.Add(5);

        Assert.Equal(("things", 5L), Assert.Single(collected.Items));
    }

    [Fact]
    public void Adversarial_A_Measurements_from_any_other_meter_do_not()
    {
        // InstrumentPublished is told about every instrument in the process, not just
        // the ones you wanted. A listener that enables all of them collects the
        // runtime's, ASP.NET Core's and EF Core's instruments too, and whatever
        // consumes it then reports numbers with nothing to do with the code under test.
        var mine = FreshMeterName();
        var theirs = FreshMeterName();
        var collected = new Collected();
        using var myMeter = new Meter(mine);
        using var theirMeter = new Meter(theirs);

        using var listener = Ex024_MeterListenerLifecycle.CreateListener(mine, collected.Add);
        myMeter.CreateCounter<long>("mine").Add(1);
        theirMeter.CreateCounter<long>("theirs").Add(99);

        Assert.Equal(("mine", 1L), Assert.Single(collected.Items));
    }

    [Fact]
    public void An_instrument_created_after_the_listener_started_is_picked_up()
    {
        // InstrumentPublished is not a one-off census at startup. A listener that only
        // enables what already existed misses everything created later, which in a real
        // application is most things.
        var name = FreshMeterName();
        var collected = new Collected();
        using var meter = new Meter(name);

        using var listener = Ex024_MeterListenerLifecycle.CreateListener(name, collected.Add);
        meter.CreateCounter<long>("created.later").Add(3);

        Assert.Equal(("created.later", 3L), Assert.Single(collected.Items));
    }

    [Fact]
    public void Adversarial_B_Observable_instruments_deliver_only_when_polled()
    {
        // Observable instruments have no other way to report. A listener that never
        // polls shows synchronous instruments only - and a gauge that never appears
        // looks exactly like a gauge that was never registered.
        var name = FreshMeterName();
        var collected = new Collected();
        using var meter = new Meter(name);
        meter.CreateObservableCounter("observed", () => 11L);

        using var listener = Ex024_MeterListenerLifecycle.CreateListener(name, collected.Add);
        Assert.Empty(collected.Items);

        listener.RecordObservableInstruments();

        Assert.Equal(("observed", 11L), Assert.Single(collected.Items));
    }

    [Fact]
    public void Adversarial_C_A_disposed_listener_receives_nothing_further()
    {
        // The one that costs money rather than data. A leaked listener keeps receiving,
        // so a second one added later means every measurement is counted twice - and
        // the dashboards are merely WRONG rather than empty. Empty gets noticed.
        var name = FreshMeterName();
        var collected = new Collected();
        using var meter = new Meter(name);
        var counter = meter.CreateCounter<long>("things");

        var listener = Ex024_MeterListenerLifecycle.CreateListener(name, collected.Add);
        counter.Add(1);
        listener.Dispose();
        counter.Add(1);

        Assert.Single(collected.Items);
    }
}
