// Exercise 052 - Migrating BackgroundWorker's ProgressChanged/RunWorkerCompleted to Task + IProgress<T> (intermediate). REFERENCE SOLUTION.
// Goal:   BackgroundWorker predates Task and IProgress<T> - its DoWork still runs on a
//         ThreadPool thread and still reports through events, but nothing says a caller has to
//         see those events: wrap it so RunAsync returns a Task<int> that completes when
//         RunWorkerCompleted fires, and forwards ProgressChanged through the IProgress<int> the
//         caller already knows how to use - never through a side channel of your own. The sharp
//         edge this row is actually about, measured directly rather than assumed (see
//         wpf/README.md): unlike row 047's Progress<T>, which captures its marshalling
//         SynchronizationContext at CONSTRUCTION time, BackgroundWorker captures it at
//         RunWorkerAsync()'s OWN call time - so RunWorkerAsync() must be called synchronously,
//         right here on RunAsync's calling thread, not from inside some other Task.Run.
// Drills: forwarding ProgressChanged through IProgress<int>.Report and only that, completing
//         the returned Task with DoWork's real return value (never early, never with a default),
//         and surfacing a DoWork exception via RunWorkerCompletedEventArgs.Error onto the Task
//         as a fault - never swallowed.

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Intermediate;

public static class Ex052_BackgroundWorkerToTask
{
    /// <summary>
    /// Runs <paramref name="doWork"/> on a BackgroundWorker (WorkerReportsProgress = true).
    /// <paramref name="doWork"/> is handed a `report` callback that forwards a percentage
    /// through BackgroundWorker.ReportProgress, and returns the operation's result. Forward
    /// every ProgressChanged through <paramref name="progress"/>.Report - the interface, never
    /// a raw event of your own. Complete the returned task from RunWorkerCompleted: with
    /// DoWork's own return value on success, or faulted with
    /// RunWorkerCompletedEventArgs.Error if DoWork threw - BackgroundWorker already catches
    /// that exception for you, so never swallow it by ignoring Error. Call RunWorkerAsync()
    /// synchronously, right here on the calling thread - not from inside another Task.Run -
    /// so wherever RunAsync itself is called from is what BackgroundWorker captures for
    /// marshalling ProgressChanged/RunWorkerCompleted back to.
    /// </summary>
    public static Task<int> RunAsync(Func<Action<int>, int> doWork, IProgress<int> progress)
    {
        var tcs = new TaskCompletionSource<int>();
        var worker = new BackgroundWorker { WorkerReportsProgress = true };

        worker.DoWork += (_, e) =>
        {
            e.Result = doWork(p => worker.ReportProgress(p));
        };

        worker.ProgressChanged += (_, e) => progress.Report(e.ProgressPercentage);

        worker.RunWorkerCompleted += (_, e) =>
        {
            if (e.Error is not null)
            {
                tcs.TrySetException(e.Error);
            }
            else
            {
                tcs.TrySetResult((int)e.Result!);
            }
        };

        worker.RunWorkerAsync();
        return tcs.Task;
    }
}
