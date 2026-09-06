using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex023_ObservableInstrumentsTests
{
    /// <summary>A queue depth the test moves by hand, counting who asked.</summary>
    private sealed class FakeQueue
    {
        public int Depth { get; set; }

        public int Reads { get; private set; }

        public int Read()
        {
            Reads++;
            return Depth;
        }
    }

    private static MeasurementProbe Probe() => new(Ex023_ObservableInstruments.MeterName);

    [Fact]
    public void Adversarial_A_Registering_the_gauge_does_not_read_anything()
    {
        // "Observable" is routinely misread as "reports automatically". Nothing is
        // automatic: the callback is invoked by whoever is collecting, on their
        // schedule, and if nobody collects it never runs at all.
        var queue = new FakeQueue { Depth = 7 };
        using var probe = Probe();

        Ex023_ObservableInstruments.RegisterQueueDepth(queue.Read);

        Assert.Equal(0, queue.Reads);
        Assert.Empty(probe.For(Ex023_ObservableInstruments.QueueDepthGauge));
    }

    [Fact]
    public void The_gauge_reports_the_current_value_once_per_poll()
    {
        var queue = new FakeQueue { Depth = 7 };
        using var probe = Probe();
        Ex023_ObservableInstruments.RegisterQueueDepth(queue.Read);

        probe.Poll();
        queue.Depth = 3;
        probe.Poll();

        // Sampling on demand is the point: a queue depth has a value at every instant,
        // and asking for it when needed is far cheaper than emitting an event each time
        // it changes.
        Assert.Equal(
            [7d, 3d],
            probe.For(Ex023_ObservableInstruments.QueueDepthGauge).Select(m => m.Value));
        Assert.Equal(2, queue.Reads);
    }

    [Fact]
    public void Granting_a_lease_moves_both_the_counter_and_the_up_down_counter()
    {
        using var probe = Probe();

        Ex023_ObservableInstruments.GrantLease();
        Ex023_ObservableInstruments.GrantLease();

        Assert.Equal(2d, probe.For(Ex023_ObservableInstruments.LeasesGrantedCounter).Sum(m => m.Value));
        Assert.Equal(2d, probe.For(Ex023_ObservableInstruments.LeasesActiveUpDown).Sum(m => m.Value));
    }

    [Fact]
    public void Adversarial_B_Releasing_a_lease_never_decrements_the_monotonic_counter()
    {
        // The distinction that breaks dashboards silently. A Counter PROMISES to be
        // monotonic and backends rely on it: they compute rates from the difference
        // between consecutive readings, so a value that goes down is not read as "minus
        // one", it is read as a process restart and the whole delta is discarded.
        //
        // "Granted ever" and "active now" are two different numbers, and wanting both
        // is normal.
        using var probe = Probe();

        Ex023_ObservableInstruments.GrantLease();
        Ex023_ObservableInstruments.GrantLease();
        Ex023_ObservableInstruments.GrantLease();
        Ex023_ObservableInstruments.ReleaseLease();

        var granted = probe.For(Ex023_ObservableInstruments.LeasesGrantedCounter);
        Assert.Equal(3, granted.Count);
        Assert.All(granted, m => Assert.True(m.Value > 0, $"a monotonic counter recorded {m.Value}"));

        Assert.Equal(2d, probe.For(Ex023_ObservableInstruments.LeasesActiveUpDown).Sum(m => m.Value));
    }

    [Fact]
    public void Adversarial_C_The_up_down_counter_actually_goes_down()
    {
        // The paired half. An implementation that simply never records the release
        // would satisfy Adversarial_B's monotonicity check perfectly and lose the
        // number the up-down counter exists for.
        using var probe = Probe();

        Ex023_ObservableInstruments.GrantLease();
        Ex023_ObservableInstruments.ReleaseLease();

        var active = probe.For(Ex023_ObservableInstruments.LeasesActiveUpDown);
        Assert.Equal(2, active.Count);
        Assert.Contains(active, m => m.Value < 0);
        Assert.Equal(0d, active.Sum(m => m.Value));
    }
}
