using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex047_ProgressReportingTests : WpfTestContext
{
    // A test double that reaches values ONLY through the interface - a bypass that raises a
    // side-channel event of its own instead of calling progress.Report leaves this empty.
    private sealed class RecordingProgress : IProgress<int>
    {
        public List<int> Values { get; } = [];
        public void Report(int value) => Values.Add(value);
    }

    private static Task NoDelay() => Task.CompletedTask;

    [WpfFact]
    public async Task Reports_Every_Step_In_Order_Through_The_Progress_Interface()
    {
        var recorder = new RecordingProgress();

        await WithTimeout(Ex047_ProgressReporting.RunAsync(3, recorder, NoDelay));

        Assert.Equal(new[] { 1, 2, 3 }, recorder.Values);
    }

    [WpfFact]
    public async Task A_Different_Step_Count_Also_Reports_Every_Step_In_Order()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var recorder = new RecordingProgress();

        await WithTimeout(Ex047_ProgressReporting.RunAsync(5, recorder, NoDelay));

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, recorder.Values);
    }

    [WpfFact]
    public async Task Zero_Steps_Reports_Nothing()
    {
        var recorder = new RecordingProgress();

        await WithTimeout(Ex047_ProgressReporting.RunAsync(0, recorder, NoDelay));

        Assert.Empty(recorder.Values);
    }

    [WpfFact]
    public async Task A_Progress_T_Built_On_The_Dispatcher_Thread_Reports_Back_On_That_Same_Thread()
    {
        // Measured directly (see wpf/README.md): Progress<T> captures the ambient
        // SynchronizationContext at CONSTRUCTION time. This Progress<int> is built right here,
        // on the dispatcher thread that [WpfFact] itself runs on.
        var dispatcherThreadId = Environment.CurrentManagedThreadId;
        var reportedThreadIds = new List<int>();
        var lastReportSeen = new TaskCompletionSource();
        var progress = new Progress<int>(value =>
        {
            reportedThreadIds.Add(Environment.CurrentManagedThreadId);
            if (value == 3)
            {
                lastReportSeen.TrySetResult();
            }
        });

        await WithTimeout(Ex047_ProgressReporting.RunAsync(3, progress, NoDelay));

        // Progress<T>.Report always POSTS - even back to the same thread it is called from -
        // so the callbacks have not necessarily run yet the instant RunAsync's own task
        // completes; wait for the last one, bounded, rather than assuming.
        await WithTimeout(lastReportSeen.Task);

        Assert.Equal(3, reportedThreadIds.Count);
        Assert.All(reportedThreadIds, id => Assert.Equal(dispatcherThreadId, id));
    }

    [WpfFact]
    public async Task A_Progress_T_Built_Off_The_Dispatcher_Thread_Reports_Somewhere_Else_Silently()
    {
        // The row's actual subject, made visible: build the SAME Progress<int> type, but on a
        // thread pool thread with no ambient SynchronizationContext - it captures THAT (the
        // thread-pool fallback), not the dispatcher, even though RunAsync below is driven from
        // the dispatcher thread same as the test above. No exception anywhere marks this -
        // it is silent by construction.
        var dispatcherThreadId = Environment.CurrentManagedThreadId;
        var reportedThreadId = -1;
        var reportSeen = new TaskCompletionSource();

        IProgress<int> progress = await Task.Run(() => new Progress<int>(value =>
        {
            reportedThreadId = Environment.CurrentManagedThreadId;
            reportSeen.TrySetResult();
        }));

        await WithTimeout(Ex047_ProgressReporting.RunAsync(1, progress, NoDelay));
        await WithTimeout(reportSeen.Task);

        Assert.NotEqual(dispatcherThreadId, reportedThreadId);
    }
}
