using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex049_SynchronizationContextCaptureTests : WpfTestContext
{
    // A genuine yield point that always finishes on a ThreadPool thread, never synchronously -
    // the Thread.Sleep guarantees Task.Run's antecedent task cannot possibly already be
    // complete by the time it is awaited, so which thread the continuation lands on is a real
    // measurement, not a race that could occasionally coincide with the dispatcher thread.
    private static Func<Task> BackgroundWork() => () => Task.Run(() => Thread.Sleep(5));

    [WpfFact]
    public async Task Keeping_The_Context_Returns_To_The_Dispatcher_Thread_After_Every_Await()
    {
        var dispatcherThreadId = Environment.CurrentManagedThreadId;

        var (afterFirst, afterSecond) = await WithTimeout(
            Ex049_SynchronizationContextCapture.RunKeepingContextAsync(BackgroundWork()));

        Assert.Equal(dispatcherThreadId, afterFirst);
        Assert.Equal(dispatcherThreadId, afterSecond);
    }

    [WpfFact]
    public async Task ConfigureAwaitFalse_Does_Not_Return_To_The_Dispatcher_Thread_After_Either_Await()
    {
        var dispatcherThreadId = Environment.CurrentManagedThreadId;

        var (afterFirst, afterSecond) = await WithTimeout(
            Ex049_SynchronizationContextCapture.RunWithConfigureAwaitFalseAsync(BackgroundWork()));

        // The ThreadPool never uses the STA dispatcher thread for anything, so this can never
        // coincidentally pass for the wrong reason.
        Assert.NotEqual(dispatcherThreadId, afterFirst);
        Assert.NotEqual(dispatcherThreadId, afterSecond);
    }

    [WpfFact]
    public async Task A_Different_Background_Operation_Still_Returns_To_The_Dispatcher_When_Context_Is_Kept()
    {
        // Varies the collaborator across call sites, per wpf/README.md's own guidance - a
        // different shape of background work, still genuinely off-thread.
        var dispatcherThreadId = Environment.CurrentManagedThreadId;
        Func<Task> work = () => Task.Run(async () => await Task.Delay(5));

        var (afterFirst, afterSecond) = await WithTimeout(
            Ex049_SynchronizationContextCapture.RunKeepingContextAsync(work));

        Assert.Equal(dispatcherThreadId, afterFirst);
        Assert.Equal(dispatcherThreadId, afterSecond);
    }
}
