using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex044_CoroutineFromTaskTests : CaliburnCoreContext
{
    // BoundedAsync/BoundedExceptionAsync (used below) live on CaliburnCoreContext - see their
    // comment for why a coroutine await needs bounding at all.

    [Fact]
    public async Task The_Coroutine_Genuinely_Waits_For_The_Task_Before_Logging_After()
    {
        var log = new List<string>();
        var beforeLogged = new TaskCompletionSource();
        var work = Task.Run(async () =>
        {
            await beforeLogged.Task;
            log.Add("task ran");
        });

        var runTask = Ex044_CoroutineFromTask.RunAsync(work, log);

        // Release work only once "before" has genuinely been logged - a deterministic readiness
        // signal instead of a fixed sleep, so this cannot flake under a loaded machine. Bounded
        // so a broken RunAsync that never logs "before" at all fails fast instead of spinning
        // forever.
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        while (log.Count == 0)
        {
            Assert.True(DateTime.UtcNow < deadline, "Timed out waiting for \"before\" to be logged.");
            await Task.Yield();
        }
        beforeLogged.SetResult();

        await BoundedAsync(runTask, "RunAsync");

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
    public async Task A_Faulted_Task_Surfaces_As_AggregateException_And_Stops_Before_Logging_After()
    {
        var log = new List<string>();
        var work = Task.Run(() => throw new InvalidOperationException("task boom"));

        // BoundedExceptionAsync (not Record.ExceptionAsync(() => BoundedAsync(...))) is what
        // makes this safe against a hang: an implementation that logs "before" and then never
        // resolves the coroutine would otherwise let a Record.ExceptionAsync wrapper silently
        // swallow the bound's own timeout failure, and "log == before" would then pass after a
        // 5-second stall instead of failing. BoundedExceptionAsync's own timeout assertion runs
        // (and can fail) before it ever touches the task's exception.
        var ex = await BoundedExceptionAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync (faulted)");

        Assert.IsType<AggregateException>(ex);
        Assert.Contains("task boom", ((AggregateException)ex!).InnerException?.Message ?? ex.Message);
        Assert.Equal(new[] { "before" }, log);
    }

    [Fact]
    public async Task A_Different_Exception_Still_Surfaces_Wrapped_With_Its_Own_Type_And_Message()
    {
        // A different exception type/message than the fact above - guards against a stub that
        // (however implausibly) special-cases one specific exception rather than genuinely
        // wrapping whatever the task actually threw.
        var log = new List<string>();
        var work = Task.Run(() => throw new ArgumentException("nope"));

        var ex = await BoundedExceptionAsync(Ex044_CoroutineFromTask.RunAsync(work, log), "RunAsync (faulted, different exception)");

        Assert.IsType<AggregateException>(ex);
        Assert.Contains("nope", ((AggregateException)ex!).InnerException?.Message ?? ex.Message);
        Assert.IsType<ArgumentException>(((AggregateException)ex).InnerException);
    }
}
