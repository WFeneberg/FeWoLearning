using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex041_CoroutineBasicsTests : CaliburnCoreContext
{
    // A step that never raises Completed makes the awaited Task wait forever, not fail - see
    // caliburn/README.md's Traps table. Bounding every coroutine await here means a forgotten
    // Completed shows up as a clear, fast failure instead of stalling the whole suite.
    private static async Task BoundedAsync(Task task, string because)
    {
        var winner = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.True(winner == task,
            $"Timed out waiting for {because} - a forgotten Completed stalls the coroutine forever instead of failing.");
        await task;
    }

    [Fact]
    public void Execute_Appends_Name_To_The_Shared_Log()
    {
        var log = new List<string>();
        var step = new Ex041_LoggingResult(log, "step-one");

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal(new[] { "step-one" }, log);
    }

    [Fact]
    public void Execute_Raises_Completed_Exactly_Once_With_Itself_As_Sender()
    {
        var log = new List<string>();
        var step = new Ex041_LoggingResult(log, "step-one");
        var raisedCount = 0;
        object? sender = null;
        step.Completed += (s, _) =>
        {
            raisedCount++;
            sender = s;
        };

        step.Execute(new CoroutineExecutionContext());

        Assert.Equal(1, raisedCount);
        Assert.Same(step, sender);
    }

    [Fact]
    public void Completed_Event_Args_Report_Success_Not_Cancelled_And_No_Error()
    {
        var log = new List<string>();
        var step = new Ex041_LoggingResult(log, "step-one");
        ResultCompletionEventArgs? args = null;
        step.Completed += (_, e) => args = e;

        step.Execute(new CoroutineExecutionContext());

        Assert.NotNull(args);
        // A stub that hard-codes cancellation or an error regardless of outcome would fail these -
        // this exercise's step never fails, so neither member should ever be set.
        Assert.False(args!.WasCancelled);
        Assert.Null(args.Error);
    }

    [Fact]
    public async Task Running_Through_TaskExtensions_ExecuteAsync_Completes_And_Logs()
    {
        var log = new List<string>();
        var step = new Ex041_LoggingResult(log, "step-one");

        await BoundedAsync(step.ExecuteAsync(), "Ex041_LoggingResult.ExecuteAsync()");

        Assert.Equal(new[] { "step-one" }, log);
    }

    [Fact]
    public void Two_Independent_Instances_Append_Their_Own_Name_Not_Each_Others()
    {
        var log = new List<string>();
        var a = new Ex041_LoggingResult(log, "a");
        var b = new Ex041_LoggingResult(log, "b");

        a.Execute(new CoroutineExecutionContext());
        b.Execute(new CoroutineExecutionContext());

        Assert.Equal(new[] { "a", "b" }, log);
    }
}
