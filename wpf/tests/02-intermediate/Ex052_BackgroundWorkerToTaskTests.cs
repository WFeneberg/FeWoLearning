using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex052_BackgroundWorkerToTaskTests : WpfTestContext
{
    // Reaches progress ONLY through the interface - a bypass that raises a side-channel event
    // of its own instead of calling progress.Report leaves this empty, same convention as
    // row 047's RecordingProgress.
    private sealed class RecordingProgress : IProgress<int>
    {
        public List<int> Values { get; } = [];
        public void Report(int value) => Values.Add(value);
    }

    [WpfFact]
    public async Task Completes_With_DoWorks_Real_Result_And_Reports_Every_Step_In_Order()
    {
        var recorder = new RecordingProgress();

        var result = await WithTimeout(Ex052_BackgroundWorkerToTask.RunAsync(report =>
        {
            report(33);
            report(66);
            report(100);
            return 7;
        }, recorder));

        // Against a bypass that hardcodes a default (0) instead of DoWork's real return value.
        Assert.Equal(7, result);
        Assert.Equal(new[] { 33, 66, 100 }, recorder.Values);
    }

    [WpfFact]
    public async Task A_Different_Result_And_Step_Count_Also_Complete_Correctly()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance - a hardcoded
        // 7/[33,66,100] from the test above cannot satisfy this one too.
        var recorder = new RecordingProgress();

        var result = await WithTimeout(Ex052_BackgroundWorkerToTask.RunAsync(report =>
        {
            report(10);
            report(50);
            return 99;
        }, recorder));

        Assert.Equal(99, result);
        Assert.Equal(new[] { 10, 50 }, recorder.Values);
    }

    [WpfFact]
    public async Task Zero_Progress_Reports_Still_Completes_With_The_Result()
    {
        var recorder = new RecordingProgress();

        var result = await WithTimeout(Ex052_BackgroundWorkerToTask.RunAsync(_ => 42, recorder));

        Assert.Equal(42, result);
        Assert.Empty(recorder.Values);
    }

    [WpfFact]
    public async Task A_DoWork_Exception_Faults_The_Task_Instead_Of_Being_Swallowed()
    {
        var recorder = new RecordingProgress();

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            WithTimeout(Ex052_BackgroundWorkerToTask.RunAsync(_ => throw new InvalidOperationException("boom"), recorder)));

        // Against a bypass that swallows the exception (catches it internally and completes
        // successfully with some default result instead of faulting the task).
        Assert.Equal("boom", thrown.Message);
    }

    [WpfFact]
    public async Task Progress_Marshals_Back_To_The_Thread_RunAsync_Was_Called_From()
    {
        // Load-bearing for the "call RunWorkerAsync() synchronously, right here" instruction:
        // measured directly (see wpf/README.md) that BackgroundWorker captures its marshalling
        // context at RunWorkerAsync()'s OWN call time, not at construction - so a bypass that
        // calls RunWorkerAsync() from inside a Task.Run wrapper instead of synchronously on
        // this calling (dispatcher) thread would report back on a pool thread instead. This
        // reaches the learner's own forwarding code directly: RelayProgress.Report runs
        // synchronously, with no marshalling of its own, so the thread it observes is exactly
        // the thread ProgressChanged itself fired on - not merely the thread a later `await`
        // happens to resume on (which the dispatcher's own SynchronizationContext would launder
        // back to the dispatcher regardless of which thread completed the task).
        var dispatcherThreadId = Environment.CurrentManagedThreadId;
        var progressThreadIds = new List<int>();

        var result = await WithTimeout(Ex052_BackgroundWorkerToTask.RunAsync(report =>
        {
            report(50);
            return 1;
        }, new RelayProgress(v => progressThreadIds.Add(Environment.CurrentManagedThreadId))));

        Assert.Equal(1, result);
        Assert.Single(progressThreadIds);
        Assert.Equal(dispatcherThreadId, progressThreadIds[0]);
    }

    private sealed class RelayProgress(Action<int> onReport) : IProgress<int>
    {
        public void Report(int value) => onReport(value);
    }
}
