// Exercise 047 - IProgress<T> and where Progress<T> actually captures its context (intermediate).
// Goal:   Report progress from a multi-step operation through the interface built for exactly
//         this - IProgress<T> - never through a side-channel event of your own. The sharp edge
//         this row is actually about, measured directly rather than assumed: Progress<T>
//         captures the SynchronizationContext that is ambient at CONSTRUCTION time, not at
//         report time - the thread that calls `new Progress<T>(...)` decides where every
//         callback runs, for the whole lifetime of that instance, regardless of which thread
//         later calls Report. One built off the UI thread posts every callback to the thread
//         pool instead, silently - there is no exception anywhere to notice this by.
// Drills: IProgress<T>.Report as the only channel progress reaches its subscriber through, and
//         Progress<T>'s construction-site capture of the ambient SynchronizationContext.
// Passes: dotnet test --filter FullyQualifiedName~Ex047_

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
    public static Task RunAsync(int steps, IProgress<int> progress, Func<Task> delay) =>
        throw new NotImplementedException("TODO: Ex047 - for step 1..steps inclusive, await delay(), then call progress.Report(step) through the IProgress<int> interface");
}
