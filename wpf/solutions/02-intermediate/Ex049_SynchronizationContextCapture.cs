// Exercise 049 - Why await returns to the UI thread, and the cost of ConfigureAwait(false) (intermediate). REFERENCE SOLUTION.
// Goal:   An `await` on a Task captures the ambient SynchronizationContext at the point it
//         suspends and marshals its continuation back through that context when the awaited
//         work finishes - which is why code after a plain `await` in a WPF method keeps running
//         on the dispatcher thread even though the awaited work itself ran elsewhere.
//         ConfigureAwait(false) skips that capture, so the continuation resumes on whatever
//         thread happened to finish the work instead - cheaper (no hop back through the
//         dispatcher's queue), but only safe when the code after the await does not touch
//         anything dispatcher-affinitized.
// Drills: a plain `await` returning to the calling SynchronizationContext across MULTIPLE
//         consecutive awaits, not just one, and ConfigureAwait(false) breaking that guarantee.

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex049_SynchronizationContextCapture
{
    /// <summary>
    /// Awaits <paramref name="work"/>() twice, back to back, WITHOUT ConfigureAwait(false) on
    /// either await, recording Environment.CurrentManagedThreadId right after each one. Because
    /// neither await drops the ambient SynchronizationContext, both recorded thread ids should
    /// be the thread this method was originally called from - a real WPF app's UI thread, the
    /// dispatcher thread in a [WpfFact] test - even though <paramref name="work"/> itself may
    /// run on some other thread entirely.
    /// </summary>
    public static async Task<(int AfterFirst, int AfterSecond)> RunKeepingContextAsync(Func<Task> work)
    {
        await work();
        var afterFirst = Environment.CurrentManagedThreadId;

        await work();
        var afterSecond = Environment.CurrentManagedThreadId;

        return (afterFirst, afterSecond);
    }

    /// <summary>
    /// Same shape as <see cref="RunKeepingContextAsync"/>, but with <c>.ConfigureAwait(false)</c>
    /// on BOTH awaits - so neither recorded thread id is guaranteed (or even likely) to match
    /// the thread this method was called from.
    /// </summary>
    public static async Task<(int AfterFirst, int AfterSecond)> RunWithConfigureAwaitFalseAsync(Func<Task> work)
    {
        await work().ConfigureAwait(false);
        var afterFirst = Environment.CurrentManagedThreadId;

        await work().ConfigureAwait(false);
        var afterSecond = Environment.CurrentManagedThreadId;

        return (afterFirst, afterSecond);
    }
}
