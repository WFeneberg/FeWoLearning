using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex044_ShutdownAndFlushTests
{
    [Fact]
    public void Adversarial_A_A_finished_span_is_not_exported_until_something_flushes()
    {
        // The window, and the point of a batch processor: exporting one span per operation
        // would put a network call on every request, so the SDK buffers and ships in
        // batches. Between the end of a span and the next batch, that span exists in one
        // process's memory and nowhere else - and a crash, a SIGKILL or a scale-in there
        // loses it.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex044_ShutdownAndFlush.BuildBatched(exported);
        Ex044_ShutdownAndFlush.DoWork();

        Assert.Empty(exported);
    }

    [Fact]
    public void ForceFlush_closes_the_window()
    {
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex044_ShutdownAndFlush.BuildBatched(exported);
        Ex044_ShutdownAndFlush.DoWork();
        provider.ForceFlush();

        Assert.Single(exported);
    }

    [Fact]
    public void Adversarial_B_Shutdown_is_final()
    {
        // The part people are surprised by. Shutdown is not a pause and there is no
        // restart: a provider that has been shut down accepts nothing more for the life of
        // the process. Calling it early "to be safe" silently ends telemetry.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using var provider = Ex044_ShutdownAndFlush.BuildBatched(exported);
        Ex044_ShutdownAndFlush.DoWork();
        provider.Shutdown();

        var afterShutdown = exported.Count;

        Ex044_ShutdownAndFlush.DoWork();
        provider.ForceFlush();

        Assert.Equal(1, afterShutdown);
        Assert.Equal(afterShutdown, exported.Count);
    }

    [Fact]
    public void Adversarial_C_A_span_still_open_when_the_provider_goes_away_is_lost()
    {
        // Why the order matters at shutdown. The spans you most want are the ones from the
        // last seconds of a process that was about to die - and a span that has not ended
        // has not been handed to any processor, so no amount of flushing can save it.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        Activity? unfinished;
        using (var provider = Ex044_ShutdownAndFlush.BuildBatched(exported))
        {
            unfinished = Ex044_ShutdownAndFlush.StartUnfinishedWork();
            Assert.NotNull(unfinished);
            provider.ForceFlush();
        }

        // Ending it now, after the provider is gone, reaches nobody.
        unfinished.Dispose();

        Assert.Empty(exported);
    }

    [Fact]
    public void Disposing_the_provider_flushes_what_was_already_finished()
    {
        // The paired half of Adversarial_C, and the reason a host that disposes its
        // provider properly does not need a separate flush for completed work.
        using var ctx = new TelemetryContext();
        var exported = new List<Activity>();

        using (var provider = Ex044_ShutdownAndFlush.BuildBatched(exported))
        {
            Ex044_ShutdownAndFlush.DoWork();
        }

        Assert.Single(exported);
    }
}
