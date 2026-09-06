using System.Windows.Threading;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex067_GlobalExceptionHooksTests : WpfTestContext
{
    // Runs `action` on the dispatcher via BeginInvoke, drains the queue, and reports whichever
    // exception (if any) escaped that Pump() call - test-local plumbing, the same "abstract
    // plumbing only in the content library, probes test-local" shape rows 040/046/057/061 follow.
    // An escaped exception here is an ordinary .NET exception propagating out of
    // Dispatcher.PushFrame - measured directly while designing this row: it fails only the ONE
    // test that triggered it, with an ordinary assertion-shaped failure, never a hang and never a
    // crash of the test host, because each [WpfFact] owns its own STA thread and Dispatcher.
    private static Exception? RunAndCaptureEscape(Dispatcher dispatcher, Action action)
    {
        try
        {
            dispatcher.BeginInvoke(action);
            Pump();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [WpfFact]
    public void Logged_Exceptions_Are_Marked_Handled_And_Never_Escape_The_Dispatcher()
    {
        var dispatcher = Dispatcher.CurrentDispatcher;
        var log = new List<Exception>();

        using (Ex067_GlobalExceptionHooks.Install(dispatcher, log))
        {
            var escaped = RunAndCaptureEscape(dispatcher, () => throw new InvalidOperationException("boom"));

            Assert.Null(escaped);
            Assert.Single(log);
            Assert.Equal("boom", log[0].Message);
        }
    }

    [WpfFact]
    public void A_Different_Exception_Is_Logged_With_Its_Own_Message()
    {
        // Vary the input across call sites: a mutant that hard-codes a fixed logged message, or
        // that logs SOMETHING without actually reading e.Exception, fails this against the test
        // above.
        var dispatcher = Dispatcher.CurrentDispatcher;
        var log = new List<Exception>();

        using (Ex067_GlobalExceptionHooks.Install(dispatcher, log))
        {
            var escaped = RunAndCaptureEscape(dispatcher, () => throw new FormatException("not-a-number"));

            Assert.Null(escaped);
            Assert.Single(log);
            Assert.Equal("not-a-number", log[0].Message);
        }
    }

    [WpfFact]
    public void The_Dispatcher_Stays_Usable_After_A_Logged_Exception()
    {
        var dispatcher = Dispatcher.CurrentDispatcher;
        var log = new List<Exception>();

        using (Ex067_GlobalExceptionHooks.Install(dispatcher, log))
        {
            var firstEscaped = RunAndCaptureEscape(dispatcher, () => throw new InvalidOperationException("first"));
            Assert.Null(firstEscaped);

            var ran = false;
            var escaped = RunAndCaptureEscape(dispatcher, () => ran = true);

            Assert.Null(escaped);
            Assert.True(ran);
        }
    }

    [WpfFact]
    public void A_Fatal_Exception_Is_Declined_By_The_Filter_And_Escapes_Unlogged()
    {
        // Rejects a mutant whose UnhandledException handler catches everything indiscriminately:
        // Ex067_FatalException must never even reach the handler, so it must escape here AND the
        // log must stay empty - a handler that ends up logging it too proves the filter never ran
        // or never actually set RequestCatch = false.
        var dispatcher = Dispatcher.CurrentDispatcher;
        var log = new List<Exception>();

        using (Ex067_GlobalExceptionHooks.Install(dispatcher, log))
        {
            var escaped = RunAndCaptureEscape(dispatcher, () => throw new Ex067_FatalException("corrupted"));

            Assert.NotNull(escaped);
            Assert.IsType<Ex067_FatalException>(escaped);
            Assert.Empty(log);
        }
    }

    [WpfFact]
    public void Disposing_The_Hook_Unsubscribes_So_Later_Exceptions_Escape_And_Stay_Unlogged()
    {
        // Rejects an "unhook nothing" Dispose: if either handler is still subscribed, this
        // ordinary exception would still be caught, logged and marked handled after Dispose - it
        // must instead escape, with the log left exactly as Dispose found it.
        var dispatcher = Dispatcher.CurrentDispatcher;
        var log = new List<Exception>();

        var hook = Ex067_GlobalExceptionHooks.Install(dispatcher, log);
        hook.Dispose();

        var escaped = RunAndCaptureEscape(dispatcher, () => throw new InvalidOperationException("post-dispose"));

        Assert.NotNull(escaped);
        Assert.Empty(log);
    }
}
