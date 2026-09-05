using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex039_CommandFromTaskTests
{
    // A synchronous-block cheat (ReactiveCommand.Create(() =>
    // _work().GetAwaiter().GetResult())) makes the calling thread deadlock
    // inside Execute() itself, waiting on a gate that only resolves on the
    // NEXT statement of that same thread - a genuine self-deadlock, not merely
    // a wrong value. Calling Execute() directly from a test body would hang
    // the whole run rather than fail it. So: run Execute() on a background
    // thread and race it against a bounded delay. If the delay wins, the
    // calling thread never returned from Execute() - fail loudly with a
    // message naming the likely cause, instead of hanging. This is the one
    // place in this file a wall-clock delay is correct: it is a failure
    // ceiling, never a synchronisation device - the gate-and-await pattern
    // below is exactly as deterministic as everywhere else in this batch.
    private static async Task<Task<string>> ExecuteWithoutBlocking(
        Ex039_CommandFromTaskViewModel vm, CancellationToken cancellationToken)
    {
        Task<string>? running = null;
        var executeReturned = Task.Run(
            () => { running = vm.FetchCommand.Execute().ToTask(cancellationToken); },
            cancellationToken);

        var winner = await Task.WhenAny(
            executeReturned,
            Task.Delay(TimeSpan.FromSeconds(5), cancellationToken));

        Assert.True(winner == executeReturned,
            "FetchCommand.Execute() did not return within 5s - it appears to block the calling " +
            "thread synchronously (e.g. via _work().Result / .Wait() / .GetAwaiter().GetResult()) " +
            "instead of genuinely awaiting _work().");

        return running!;
    }

    // The deterministic gate pattern: the async body only completes when the
    // test says so, so "mid-flight" is a state under our control, never a race.
    [Fact]
    public async Task Executing_Genuinely_Awaits_The_Task_Before_The_Result_Is_Observed()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex039_CommandFromTaskViewModel(() => gate.Task);

        var running = await ExecuteWithoutBlocking(vm, TestContext.Current.CancellationToken);

        // Not resolved yet: a cheat that fabricates a result instead of
        // awaiting _work() would already show it here.
        Assert.False(running.IsCompleted);
        Assert.Equal(string.Empty, vm.Result);

        gate.SetResult("async-result");
        var awaited = await running;

        Assert.Equal("async-result", awaited);
        Assert.Equal("async-result", vm.Result);
    }

    // A second, differently-valued run against a fresh gate: guards against a
    // command that reproduces one hard-coded string regardless of _work.
    [Fact]
    public async Task A_Second_Distinctly_Valued_Run_Flows_Through_Just_As_Well()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex039_CommandFromTaskViewModel(() => gate.Task);

        var running = await ExecuteWithoutBlocking(vm, TestContext.Current.CancellationToken);
        gate.SetResult("a completely different value");
        await running;

        Assert.Equal("a completely different value", vm.Result);
    }

    // Structural check: Result must come from the command's own emitted value,
    // not from some side path that merely happens to agree with it.
    [Fact]
    public async Task The_Commands_Own_Emission_Matches_What_Result_Stores()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex039_CommandFromTaskViewModel(() => gate.Task);
        string? emitted = null;
        using var sub = vm.FetchCommand.Subscribe(v => emitted = v);

        var running = await ExecuteWithoutBlocking(vm, TestContext.Current.CancellationToken);
        gate.SetResult("emitted-and-stored");
        await running;

        Assert.Equal("emitted-and-stored", emitted);
        Assert.Equal(emitted, vm.Result);
    }
}
