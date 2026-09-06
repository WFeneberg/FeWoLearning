using System.Windows.Threading;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex059_DispatcherLatencyMetricTests
{
    /// <summary>Long enough to be unmistakable, short enough not to slow the suite.</summary>
    private static readonly TimeSpan Slow = TimeSpan.FromMilliseconds(150);

    [WpfFact]
    public async Task Posting_work_records_one_latency_tagged_with_its_priority()
    {
        using var probe = new MeasurementProbe(Ex059_DispatcherLatencyMetric.MeterName);
        var ran = false;

        await Ex059_DispatcherLatencyMetric.PostMeasuredAsync(
            Dispatcher.CurrentDispatcher, DispatcherPriority.Normal, () => ran = true);

        Assert.True(ran, "the work has to actually run");

        var measurement = Assert.Single(
            probe.For(Ex059_DispatcherLatencyMetric.LatencyHistogram));
        Assert.Equal(
            nameof(DispatcherPriority.Normal),
            measurement.Tag(Ex059_DispatcherLatencyMetric.PriorityTag));
        Assert.Equal(
            Ex059_DispatcherLatencyMetric.LatencyUnit,
            probe.UnitOf(Ex059_DispatcherLatencyMetric.LatencyHistogram));
    }

    [WpfFact]
    public async Task Adversarial_A_Work_queued_behind_something_slow_records_a_long_wait()
    {
        // Queue latency is how long a piece of work WAITED before the UI thread got to
        // it. This is the case it exists to catch: nothing here is slow except the thing
        // in front of it, and the user perceives exactly this number.
        using var probe = new MeasurementProbe(Ex059_DispatcherLatencyMetric.MeterName);
        var dispatcher = Dispatcher.CurrentDispatcher;

        // Blocks the UI thread, exactly as a synchronous handler would.
        var blocker = dispatcher.BeginInvoke(DispatcherPriority.Normal, () => Thread.Sleep(Slow));

        var measured = Ex059_DispatcherLatencyMetric.PostMeasuredAsync(
            dispatcher, DispatcherPriority.Normal, () => { });

        await blocker;
        await measured;

        var latency = Assert.Single(probe.For(Ex059_DispatcherLatencyMetric.LatencyHistogram)).Value;
        Assert.True(
            latency >= Slow.TotalMilliseconds * 0.6,
            $"the queue was blocked for {Slow.TotalMilliseconds}ms and the latency says {latency}ms");
    }

    [WpfFact]
    public async Task Adversarial_B_Work_that_is_itself_slow_records_a_short_wait()
    {
        // The same measurement from the other side, and the pair is the row. A long wait
        // means the UI thread is busy with something else; a long run means this work is
        // expensive. Different causes, different fixes - and a metric that moves for both
        // reasons tells you about neither.
        using var probe = new MeasurementProbe(Ex059_DispatcherLatencyMetric.MeterName);

        await Ex059_DispatcherLatencyMetric.PostMeasuredAsync(
            Dispatcher.CurrentDispatcher, DispatcherPriority.Normal, () => Thread.Sleep(Slow));

        var latency = Assert.Single(probe.For(Ex059_DispatcherLatencyMetric.LatencyHistogram)).Value;
        Assert.True(
            latency < Slow.TotalMilliseconds * 0.5,
            $"the work took {Slow.TotalMilliseconds}ms and the WAIT should not include it, but says {latency}ms");
    }

    [WpfFact]
    public async Task Adversarial_C_The_priority_separates_the_series()
    {
        // A dimension rather than separate instruments, for row 021's reason: a small
        // bounded set, and you want "how long is the queue" across all of it and "which
        // priority is starving" within it, from one series.
        using var probe = new MeasurementProbe(Ex059_DispatcherLatencyMetric.MeterName);
        var dispatcher = Dispatcher.CurrentDispatcher;

        await Ex059_DispatcherLatencyMetric.PostMeasuredAsync(
            dispatcher, DispatcherPriority.Normal, () => { });
        await Ex059_DispatcherLatencyMetric.PostMeasuredAsync(
            dispatcher, DispatcherPriority.Background, () => { });

        var priorities = probe.For(Ex059_DispatcherLatencyMetric.LatencyHistogram)
            .Select(m => m.Tag(Ex059_DispatcherLatencyMetric.PriorityTag) ?? "<null>")
            .ToArray();

        Assert.Equal(
            [nameof(DispatcherPriority.Normal), nameof(DispatcherPriority.Background)],
            priorities);
    }
}
