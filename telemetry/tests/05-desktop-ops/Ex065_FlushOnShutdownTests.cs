using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex065_FlushOnShutdownTests
{
    [Fact]
    public void Pending_starts_at_zero()
    {
        // Zero rather than unknown. A counter that only becomes meaningful after the first
        // record cannot be put on a dashboard, because nobody can tell "nothing pending"
        // from "not measuring yet".
        using var ctx = new TelemetryContext();
        var delivered = new List<Activity>();

        using var provider = Ex065_FlushOnShutdown.Build(delivered);

        Assert.Equal(0, Ex065_FlushOnShutdown.Pending);
    }

    [Fact]
    public void Adversarial_A_Finished_work_that_has_not_been_flushed_is_counted()
    {
        // Row 044 established that the window exists. This makes it a number the
        // application can see - which is the whole difference between "did we flush" and
        // "how much would we have lost".
        using var ctx = new TelemetryContext();
        var delivered = new List<Activity>();

        using var provider = Ex065_FlushOnShutdown.Build(delivered);
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();

        Assert.Equal(3, Ex065_FlushOnShutdown.Pending);
        Assert.Empty(delivered);
    }

    [Fact]
    public void Flushing_delivers_the_pending_work_and_returns_the_count_to_zero()
    {
        using var ctx = new TelemetryContext();
        var delivered = new List<Activity>();

        using var provider = Ex065_FlushOnShutdown.Build(delivered);
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();

        provider.ForceFlush();

        Assert.Equal(0, Ex065_FlushOnShutdown.Pending);
        Assert.Equal(2, delivered.Count);
    }

    [Fact]
    public void Adversarial_B_An_exit_without_flushing_loses_exactly_Pending_records()
    {
        // What makes the count worth having. If Pending is usually 3, an ungraceful exit
        // costs three records and nobody needs to care. If it is usually 4000 because the
        // schedule is five minutes, every crash loses five minutes of evidence about the
        // crash - and the fix is a shorter schedule, which you would never have known to
        // make.
        //
        // "Exit" here is abandoning the provider without flushing or disposing, which is
        // the closest a test can honestly get to a laptop lid closing.
        using var ctx = new TelemetryContext();
        var delivered = new List<Activity>();

        var provider = Ex065_FlushOnShutdown.Build(delivered);
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();
        Ex065_FlushOnShutdown.DoWork();

        var wouldBeLost = Ex065_FlushOnShutdown.Pending;

        Assert.Equal(4, wouldBeLost);
        Assert.Empty(delivered);

        // Cleaned up here rather than left to a finalizer, so the next test starts from a
        // known state - but everything above already happened.
        provider.Dispose();
    }

    [Fact]
    public void Adversarial_C_Shutdown_after_a_flush_delivers_nothing_further()
    {
        // The paired half, and it catches a counter that reports work as pending forever:
        // if Pending never falls, "how much would we lose" is always the whole run and the
        // number is useless.
        using var ctx = new TelemetryContext();
        var delivered = new List<Activity>();

        using var provider = Ex065_FlushOnShutdown.Build(delivered);
        Ex065_FlushOnShutdown.DoWork();
        provider.ForceFlush();

        var afterFlush = delivered.Count;
        provider.Shutdown();

        Assert.Equal(1, afterFlush);
        Assert.Equal(afterFlush, delivered.Count);
        Assert.Equal(0, Ex065_FlushOnShutdown.Pending);
    }
}
