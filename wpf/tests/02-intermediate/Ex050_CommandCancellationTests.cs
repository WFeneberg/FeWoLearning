using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex050_CommandCancellationTests : WpfTestContext
{
    // A run whose completion (or cancellation) the test controls: it awaits a gate the test
    // holds, and registers on the token it is actually given so a real Cancel() call reaches it.
    private sealed class CancellableProbe : Ex050_CancellableCommandBase
    {
        public TaskCompletionSource? Gate;
        public int Invocations;

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Invocations++;
            using var registration = cancellationToken.Register(() => Gate?.TrySetCanceled(cancellationToken));
            await Gate!.Task;
        }
    }

    private sealed class ThrowingProbe : Ex050_CancellableCommandBase
    {
        protected override Task RunAsync(CancellationToken cancellationToken)
            => Task.FromException(new InvalidOperationException("boom"));
    }

    [WpfFact]
    public void Starts_Out_Executable_And_Idle()
    {
        var command = new CancellableProbe();

        Assert.True(command.CanExecute(null));
        Assert.False(command.IsExecuting);
        Assert.False(command.WasCancelled);
        Assert.Null(command.LastError);
    }

    [WpfFact]
    public void Cancel_With_Nothing_Running_Is_A_No_Op()
    {
        var command = new CancellableProbe();

        var exception = Record.Exception(() => command.Cancel());

        Assert.Null(exception);
        Assert.False(command.WasCancelled);
        Assert.False(command.IsExecuting);
    }

    [WpfFact]
    public async Task Cancel_Stops_The_Currently_Running_Operation()
    {
        var command = new CancellableProbe { Gate = new TaskCompletionSource() };
        var run = command.ExecuteAsync(null);

        Assert.True(command.IsExecuting);

        command.Cancel(); // synchronous - see wpf/README.md on await CancelAsync()

        await WithTimeout(run);

        // Cancellation is the NORMAL outcome here - not an error - and must leave the command
        // idle and ready to go again, same as any other completed run.
        Assert.True(command.WasCancelled);
        Assert.Null(command.LastError);
        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute(null));
    }

    [WpfFact]
    public async Task Can_Run_Again_After_Being_Cancelled_And_WasCancelled_Resets()
    {
        var command = new CancellableProbe { Gate = new TaskCompletionSource() };
        var first = command.ExecuteAsync(null);
        command.Cancel();
        await WithTimeout(first);
        Assert.True(command.WasCancelled);

        // Load-bearing against a STALE CancellationTokenSource (one created once and reused
        // across runs instead of a fresh one per run): start the second run BEFORE completing
        // its gate, not after. A leftover, already-cancelled token would cancel this run's
        // registration the instant RunAsync registers on it - synchronously, before this method
        // ever reaches SetResult below - so completing the gate afterward is what actually
        // proves the second run started on an uncancelled token of its own.
        command.Gate = new TaskCompletionSource();
        var second = command.ExecuteAsync(null);
        command.Gate.SetResult(); // this run completes normally, uncancelled

        await WithTimeout(second);

        Assert.False(command.WasCancelled);
        Assert.Equal(2, command.Invocations);
    }

    [WpfFact]
    public async Task Cancel_Targets_The_Current_Run_Even_After_An_Earlier_Run_Already_Finished()
    {
        var command = new CancellableProbe { Gate = new TaskCompletionSource() };
        command.Gate.SetResult();
        await WithTimeout(command.ExecuteAsync(null)); // first run: completes normally
        Assert.False(command.WasCancelled);

        command.Gate = new TaskCompletionSource();
        var second = command.ExecuteAsync(null);
        command.Cancel();

        // Against an implementation that keeps a single CancellationTokenSource reference
        // instead of the CURRENT run's: either this throws reaching a stale, disposed source,
        // or it silently does nothing and this hangs (bounded below, so it fails instead).
        await WithTimeout(second);

        Assert.True(command.WasCancelled);
        Assert.Equal(2, command.Invocations);
    }

    [WpfFact]
    public async Task A_Genuine_Failure_Sets_LastError_Not_WasCancelled()
    {
        var command = new ThrowingProbe();

        await WithTimeout(command.ExecuteAsync(null));

        Assert.False(command.WasCancelled);
        Assert.IsType<InvalidOperationException>(command.LastError);
    }

    [WpfFact]
    public async Task Refuses_A_Second_Concurrent_Run()
    {
        var command = new CancellableProbe { Gate = new TaskCompletionSource() };
        var first = command.ExecuteAsync(null);
        var second = command.ExecuteAsync(null);

        Assert.True(second.IsCompleted);
        Assert.Equal(1, command.Invocations);

        command.Cancel();
        await WithTimeout(first);
        await WithTimeout(second);
    }

    [WpfFact]
    public async Task Announces_Both_Edges_Around_A_Cancelled_Run()
    {
        var command = new CancellableProbe { Gate = new TaskCompletionSource() };
        var canExecuteChangedCount = 0;
        command.CanExecuteChanged += (_, _) => canExecuteChangedCount++;

        var run = command.ExecuteAsync(null);
        Assert.Equal(1, canExecuteChangedCount);

        command.Cancel();
        await WithTimeout(run);

        Assert.Equal(2, canExecuteChangedCount);
    }
}
