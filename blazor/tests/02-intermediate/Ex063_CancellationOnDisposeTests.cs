using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex063_CancellationOnDisposeTests : BunitContext
{
    private static TaskCompletionSource<string> Gate()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public void Runs_The_Load_Under_A_Live_Token()
    {
        var gate = Gate();
        var captured = CancellationToken.None;

        Render<Ex063_CancellationOnDispose>(p => p.Add(c => c.Load, ct =>
        {
            captured = ct;
            return gate.Task;
        }));

        Assert.True(captured.CanBeCanceled); // CancellationToken.None cannot
        Assert.False(captured.IsCancellationRequested);
    }

    // Non-vacuity for the cancellation facts below: the happy path must still work,
    // which rules out a component that cancels its own token straight away.
    [Fact]
    public void Renders_The_Result_When_The_Load_Finishes_First()
    {
        var gate = Gate();
        var cut = Render<Ex063_CancellationOnDispose>(p => p.Add(c => c.Load, _ => gate.Task));

        gate.SetResult("done");

        cut.WaitForAssertion(() => Assert.Equal("done", cut.Find("#result").TextContent));
    }

    [Fact]
    public async Task Disposing_Cancels_The_Token_The_Load_Is_Holding()
    {
        var gate = Gate();
        var captured = CancellationToken.None;
        Render<Ex063_CancellationOnDispose>(p => p.Add(c => c.Load, ct =>
        {
            captured = ct;
            return gate.Task;
        }));

        await DisposeComponentsAsync();

        Assert.True(captured.IsCancellationRequested);
    }

    // Ruling: the load finishing after disposal is the race this exercise exists for.
    // Draining the dispatcher afterwards is what makes the negative assertion mean
    // something - the component's continuation is queued on it, so once a no-op has
    // run to completion there, the continuation has already had its turn.
    [Fact]
    public async Task A_Load_That_Finishes_After_Disposal_Does_Not_Write_The_Result()
    {
        var gate = Gate();
        var cut = Render<Ex063_CancellationOnDispose>(p => p.Add(c => c.Load, _ => gate.Task));
        var instance = cut.Instance;

        await DisposeComponentsAsync();
        gate.SetResult("late");
        await Renderer.Dispatcher.InvokeAsync(() => { });

        Assert.Equal("", instance.Result);
    }
}
