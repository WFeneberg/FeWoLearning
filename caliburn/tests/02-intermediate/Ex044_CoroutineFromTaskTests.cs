using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex044_CoroutineFromTaskTests : CaliburnCoreContext
{
    // A step that never raises Completed makes the awaited Task wait forever, not fail - see
    // caliburn/README.md's Traps table. Bounding every coroutine await here means a broken
    // adapter shows up as a clear, fast failure instead of stalling the whole suite.
    private static async Task BoundedAsync(Task task, string because)
    {
        var winner = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.True(winner == task,
            $"Timed out waiting for {because} - a forgotten wait stalls the coroutine forever instead of failing.");
        await task;
    }

    private static async Task<T> BoundedAsync<T>(Task<T> task, string because)
    {
        var winner = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.True(winner == task,
            $"Timed out waiting for {because} - a forgotten wait stalls the coroutine forever instead of failing.");
        return await task;
    }

    [Fact]
    public async Task The_Coroutine_Genuinely_Waits_For_The_Task_Before_Logging_After()
    {
        var log = new List<string>();
        var work = Task.Run(async () =>
        {
            await Task.Delay(50);
            log.Add("task ran");
        });

        await BoundedAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync");

        Assert.Equal(new[] { "before", "task ran", "after" }, log);
    }

    [Fact]
    public async Task An_Already_Completed_Task_Still_Logs_Around_It_In_Order()
    {
        var log = new List<string>();
        var work = Task.CompletedTask;

        await BoundedAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync");

        Assert.Equal(new[] { "before", "after" }, log);
    }

    [Fact]
    public async Task Task_Of_T_AsResult_Hands_Back_The_Tasks_Value_Through_Result()
    {
        var work = Task.FromResult(123);

        var value = await BoundedAsync(Ex044_CoroutineFromTask.RunCaptureAsync(work), "RunCaptureAsync");

        Assert.Equal(123, value);
    }

    [Fact]
    public async Task A_Faulted_Task_Surfaces_As_AggregateException_Not_The_Bare_Original()
    {
        var log = new List<string>();
        var work = Task.Run(() => throw new InvalidOperationException("task boom"));

        var ex = await Record.ExceptionAsync(() => BoundedAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync (faulted)"));

        Assert.IsType<AggregateException>(ex);
        Assert.Contains("task boom", ((AggregateException)ex!).InnerException?.Message ?? ex.Message);
    }

    [Fact]
    public async Task A_Faulted_Task_Stops_The_Sequence_Before_Logging_After()
    {
        var log = new List<string>();
        var work = Task.Run(() => throw new InvalidOperationException("task boom"));

        await Record.ExceptionAsync(() => BoundedAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync (faulted)"));

        Assert.Equal(new[] { "before" }, log);
    }
}
