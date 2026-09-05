// Exercise 047 - IProgress<T> and where Progress<T> actually captures its context (intermediate). REFERENCE SOLUTION.
// Goal:   Report progress from a multi-step operation through the interface built for exactly
//         this - IProgress<T> - never through a side-channel event of your own. The sharp edge
//         this row is actually about, measured directly rather than assumed: Progress<T>
//         captures the SynchronizationContext that is ambient at CONSTRUCTION time, not at
//         report time - the thread that calls `new Progress<T>(...)` decides where every
//         callback runs, for the whole lifetime of that instance, regardless of which thread
//         later calls Report. One built off the UI thread posts every callback to the thread
//         pool instead, silently - there is no exception anywhere to notice this by.
// Drills: IProgress<T>.Report as the only channel progress reaches its subscriber through, and
//         WHERE you construct a Progress<T> - not merely accepting one someone else already
//         built - deciding which thread it marshals its callback onto.

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex047_ProgressReporting
{
    /// <summary>
    /// Runs <paramref name="steps"/> increments, numbered 1..<paramref name="steps"/>
    /// inclusive: for each one, await <paramref name="delay"/>() (a caller-supplied way to
    /// wait - in an app, something like <c>() =&gt; Task.Delay(50)</c>; in a test, something
    /// the test controls), then report that step's number through
    /// <paramref name="progress"/> - via IProgress&lt;int&gt;.Report, and ONLY that; never by
    /// raising an event or calling any concrete <see cref="Progress{T}"/> member directly, since
    /// the whole point is that the caller's own progress object - whatever it actually is -
    /// is what decides how (and on which thread) that value is delivered.
    /// </summary>
    public static async Task RunAsync(int steps, IProgress<int> progress, Func<Task> delay)
    {
        for (var step = 1; step <= steps; step++)
        {
            await delay();
            progress.Report(step);
        }
    }

    /// <summary>
    /// Same step loop, but the caller hands over a plain callback (<paramref name="onProgress"/>)
    /// instead of an already-built <see cref="IProgress{T}"/> - and the stepping itself happens
    /// via real background work (<see cref="Task.Run(Func{Task})"/>), not just an awaited delay.
    /// Where you build the <see cref="Progress{T}"/> (of int) wrapping <paramref name="onProgress"/>
    /// now actually matters: build it on the thread this method is CALLED from, before starting
    /// the background work - not inside it - so it captures the caller's ambient
    /// SynchronizationContext instead of whatever pool thread ends up running the loop.
    /// </summary>
    public static Task RunOnBackgroundAsync(int steps, Action<int> onProgress, Func<Task> delay)
    {
        IProgress<int> progress = new Progress<int>(onProgress); // built HERE - on the calling thread

        return Task.Run(async () =>
        {
            for (var step = 1; step <= steps; step++)
            {
                await delay();
                progress.Report(step);
            }
        });
    }
}
