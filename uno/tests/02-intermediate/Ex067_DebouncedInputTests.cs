using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex067_DebouncedInputTests : UnoTestContext
{
    /// <summary>
    /// A delay the test controls: it hands out a task per call that the test releases when
    /// it wants the debounce window to elapse. No wall clock, so nothing is slow or flaky.
    /// </summary>
    private sealed class ControlledDelay
    {
        private readonly List<TaskCompletionSource> _pending = [];

        public int Started => _pending.Count;

        public Task Wait(CancellationToken ct)
        {
            var gate = new TaskCompletionSource();
            _pending.Add(gate);
            ct.Register(() => gate.TrySetCanceled(ct));
            return gate.Task;
        }

        /// <summary>Releases the most recent delay, as if its window had elapsed.</summary>
        public void Elapse() => _pending[^1].TrySetResult();
    }

    private static (Ex067_DebouncedInput Input, ControlledDelay Delay) Debounced()
    {
        var delay = new ControlledDelay();
        Ex067_DebouncedInput? input = null;
        input = new Ex067_DebouncedInput(delay.Wait, (_, _) => Task.CompletedTask);
        return (input, delay);
    }

    [Fact]
    public async Task Nothing_Runs_Before_The_Delay_Elapses()
    {
        var (input, delay) = Debounced();

        var change = input.ChangeAsync("a");

        Assert.Equal(0, input.Runs);
        Assert.False(change.IsCompleted);

        // Settled before the test ends. xunit waits for the async operations a test
        // started, so an unreleased gate hangs the whole test host - see ex049.
        delay.Elapse();
        await change;
    }

    [Fact]
    public async Task The_Action_Runs_Once_The_Delay_Elapses()
    {
        var (input, delay) = Debounced();
        var change = input.ChangeAsync("a");

        delay.Elapse();
        await change;

        Assert.Equal(1, input.Runs);
        Assert.Equal("a", input.LastValue);
    }

    [Fact]
    public async Task A_Burst_Runs_The_Action_Once()
    {
        var (input, delay) = Debounced();

        var first = input.ChangeAsync("a");
        var second = input.ChangeAsync("ab");
        var third = input.ChangeAsync("abc");
        delay.Elapse();
        await Task.WhenAll(first, second, third);

        // Three keystrokes, one search. Without the cancellation each one would fire.
        Assert.Equal(1, input.Runs);
    }

    [Fact]
    public async Task The_Last_Value_Of_A_Burst_Wins()
    {
        var (input, delay) = Debounced();

        var first = input.ChangeAsync("a");
        var second = input.ChangeAsync("abc");
        delay.Elapse();
        await Task.WhenAll(first, second);

        Assert.Equal("abc", input.LastValue);
    }

    [Fact]
    public async Task Each_Change_Starts_A_New_Window()
    {
        var (input, delay) = Debounced();

        var first = input.ChangeAsync("a");
        var second = input.ChangeAsync("ab");
        delay.Elapse();
        await Task.WhenAll(first, second);

        Assert.Equal(2, delay.Started);
    }

    [Fact]
    public async Task Two_Settled_Changes_Run_Twice()
    {
        var (input, delay) = Debounced();

        var first = input.ChangeAsync("a");
        delay.Elapse();
        await first;

        var second = input.ChangeAsync("b");
        delay.Elapse();
        await second;

        Assert.Equal(2, input.Runs);
        Assert.Equal("b", input.LastValue);
    }

    [Fact]
    public async Task A_Cancelled_Change_Does_Not_Throw_Out_Of_ChangeAsync()
    {
        var (input, delay) = Debounced();

        var first = input.ChangeAsync("a");
        var second = input.ChangeAsync("ab");
        delay.Elapse();
        await second;

        // The delay throws OperationCanceledException for the superseded change. That is
        // the normal path here, and it must not surface as a failure to the caller.
        await first;
        Assert.False(first.IsFaulted);
    }

    [Fact]
    public async Task The_Action_Sees_The_Value_It_Was_Called_With()
    {
        var delay = new ControlledDelay();
        var seen = new List<string>();
        var input = new Ex067_DebouncedInput(delay.Wait, (value, _) =>
        {
            seen.Add(value);
            return Task.CompletedTask;
        });

        var change = input.ChangeAsync("query");
        delay.Elapse();
        await change;

        Assert.Equal(["query"], seen);
    }
}
