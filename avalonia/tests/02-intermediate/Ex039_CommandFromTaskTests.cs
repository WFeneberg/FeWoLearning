using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex039_CommandFromTaskTests
{
    // The deterministic gate pattern: the async body only completes when the
    // test says so, so "mid-flight" is a state under our control, never a race.
    [Fact]
    public async Task Executing_Genuinely_Awaits_The_Task_Before_The_Result_Is_Observed()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex039_CommandFromTaskViewModel(() => gate.Task);

        var running = vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken);

        // Not resolved yet: a cheat that calls _work().Result / .Wait() inline
        // would have blocked the calling thread right here, never reaching this
        // line at all, because gate is still pending. A cheat that fabricates a
        // result instead of awaiting _work() would already show it here.
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

        var running = vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken);
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

        var running = vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken);
        gate.SetResult("emitted-and-stored");
        await running;

        Assert.Equal("emitted-and-stored", emitted);
        Assert.Equal(emitted, vm.Result);
    }
}
