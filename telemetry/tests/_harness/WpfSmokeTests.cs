using System.Windows.Threading;

namespace FeWoLearning.Telemetry.Tests.Harness;

/// <summary>
/// Canaries for block <c>05-desktop-ops</c>, which is the only part of this track that
/// needs an STA thread. They pass in BOTH modes and fail first when the
/// <c>Xunit.StaFact</c> pin breaks.
///
/// Measured 2026-09-06: none of these needs an interactive desktop session - unlike
/// <c>caliburn/</c>, which does, because it opens a real Window. No row in this block is
/// supposed to open one.
/// </summary>
public class WpfSmokeTests
{
    [WpfFact]
    public void A_WpfFact_runs_on_an_sta_thread()
    {
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
    }

    [WpfFact]
    public void A_WpfFact_has_a_live_dispatcher()
    {
        // The Dispatcher is what block 05's rows measure and hook. Without one, every
        // row there would fail for a reason that has nothing to do with telemetry.
        Assert.NotNull(Dispatcher.CurrentDispatcher);
        Assert.False(Dispatcher.CurrentDispatcher.HasShutdownStarted);
    }

    [WpfFact]
    public async Task Awaiting_resumes_on_the_dispatcher()
    {
        // StaFact installs a DispatcherSynchronizationContext, so a continuation comes
        // back to the UI thread exactly as it would in a real application. A row that
        // measured queue latency without this would be measuring a thread-pool hop.
        var before = Dispatcher.CurrentDispatcher;

        await Task.Yield();

        Assert.Same(before, Dispatcher.CurrentDispatcher);
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
    }
}
