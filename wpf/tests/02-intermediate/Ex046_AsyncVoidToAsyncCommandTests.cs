using System.ComponentModel;
using System.Windows.Input;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex046_AsyncVoidToAsyncCommandTests : WpfTestContext
{
    // A run whose completion the test controls, so "still running" and "just finished" are
    // both observable without any wall clock.
    private sealed class GatedCommand : Ex046_AsyncCommandBase
    {
        public TaskCompletionSource? Gate;
        public int Invocations;

        protected override async Task RunAsync(object? parameter)
        {
            Invocations++;
            if (Gate is not null)
            {
                await Gate.Task;
            }
        }
    }

    private sealed class ThrowingCommand : Ex046_AsyncCommandBase
    {
        public Func<Exception?> FailureFactory = () => null;

        protected override Task RunAsync(object? parameter)
        {
            var failure = FailureFactory();
            return failure is null ? Task.CompletedTask : Task.FromException(failure);
        }
    }

    private static Task WaitForIdleAsync(Ex046_AsyncCommandBase command)
    {
        if (!command.IsExecuting)
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource();
        PropertyChangedEventHandler? handler = null;
        handler = (_, e) =>
        {
            if (e.PropertyName != nameof(Ex046_AsyncCommandBase.IsExecuting) || command.IsExecuting) return;
            command.PropertyChanged -= handler;
            tcs.TrySetResult();
        };
        command.PropertyChanged += handler;
        return tcs.Task;
    }

    [WpfFact]
    public void Starts_Out_Executable_And_Idle()
    {
        var command = new GatedCommand();

        Assert.True(command.CanExecute(null));
        Assert.False(command.IsExecuting);
        Assert.Null(command.LastError);
        Assert.Equal(0, command.RunCount);
    }

    [WpfFact]
    public async Task ExecuteAsyncs_Returned_Task_Only_Completes_Once_The_Real_Work_Does()
    {
        var command = new GatedCommand { Gate = new TaskCompletionSource() };
        var run = command.ExecuteAsync(null);

        // Against a bypass that fires the work off some other way (an async void body
        // underneath) and hands back an already-finished task regardless: this would already
        // be true here, before the gate is ever released.
        Assert.False(run.IsCompleted, "ExecuteAsync's task completed before the gated operation did");
        Assert.True(command.IsExecuting);

        // Load-bearing for CanExecute's own TODO half ("false while IsExecuting is true"): a
        // CanExecute that ignores IsExecuting and always answers true would pass every OTHER
        // test in this file, since none of them call CanExecute while a run is in flight.
        Assert.False(command.CanExecute(null));

        command.Gate!.SetResult();
        await WithTimeout(run);

        Assert.True(run.IsCompletedSuccessfully);
        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute(null));
    }

    [WpfFact]
    public async Task Refuses_A_Second_Concurrent_Run_And_Its_Task_Is_Already_Complete()
    {
        var command = new GatedCommand { Gate = new TaskCompletionSource() };
        var first = command.ExecuteAsync(null);

        var second = command.ExecuteAsync(null);

        // A refusal, not a queue: the second call's own task needs no further waiting, and
        // RunAsync must not have started a second time.
        Assert.True(second.IsCompleted);
        Assert.Equal(1, command.Invocations);

        command.Gate!.SetResult();
        await WithTimeout(first);
        await WithTimeout(second);

        Assert.Equal(1, command.Invocations);
    }

    [WpfFact]
    public async Task Announces_Both_Edges_Of_IsExecuting_And_CanExecuteChanged()
    {
        var command = new GatedCommand { Gate = new TaskCompletionSource() };
        var canExecuteChangedCount = 0;
        var isExecutingRaises = new List<bool>();
        command.CanExecuteChanged += (_, _) => canExecuteChangedCount++;
        command.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Ex046_AsyncCommandBase.IsExecuting))
            {
                isExecutingRaises.Add(command.IsExecuting);
            }
        };

        var run = command.ExecuteAsync(null);

        // Announcing only the disable leaves a bound button greyed out forever - both edges
        // must fire.
        Assert.Equal(1, canExecuteChangedCount);

        command.Gate!.SetResult();
        await WithTimeout(run);

        Assert.Equal(2, canExecuteChangedCount);
        Assert.Equal(new[] { true, false }, isExecutingRaises);
    }

    [WpfFact]
    public async Task Captures_A_Failure_Instead_Of_Letting_It_Escape()
    {
        var boom = new InvalidOperationException("boom");
        var command = new ThrowingCommand { FailureFactory = () => boom };

        await WithTimeout(command.ExecuteAsync(null));

        Assert.Same(boom, command.LastError);
    }

    [WpfFact]
    public async Task IsExecuting_Is_Cleared_On_The_Failure_Path_Too()
    {
        var command = new ThrowingCommand { FailureFactory = () => new InvalidOperationException() };

        await WithTimeout(command.ExecuteAsync(null));

        // Against a bypass that sets IsExecuting but only resets it on the success path: one
        // failure would disable the command permanently.
        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute(null));
    }

    [WpfFact]
    public async Task Can_Run_Again_After_A_Failure_And_The_Error_Clears()
    {
        var fail = true;
        var command = new ThrowingCommand { FailureFactory = () => fail ? new InvalidOperationException("first") : null };

        await WithTimeout(command.ExecuteAsync(null));
        Assert.NotNull(command.LastError);

        fail = false;
        await WithTimeout(command.ExecuteAsync(null));

        Assert.Null(command.LastError);
        Assert.Equal(2, command.RunCount);
    }

    [WpfFact]
    public async Task A_Second_Different_Operation_Is_Also_Refused_While_The_First_Runs()
    {
        // Varies the collaborator across the two concurrent attempts, not just the parameter -
        // the gate belongs to ONE command instance regardless of which operation shape it runs.
        var command = new GatedCommand { Gate = new TaskCompletionSource() };
        var first = command.ExecuteAsync("alpha");
        var second = command.ExecuteAsync("beta");

        command.Gate!.SetResult();
        await WithTimeout(first);
        await WithTimeout(second);

        Assert.Equal(1, command.Invocations);
        Assert.Equal(1, command.RunCount);
    }

    [WpfFact]
    public async Task The_ICommand_Boundary_Still_Runs_The_Operation_Through_The_Same_Path()
    {
        var command = new GatedCommand { Gate = new TaskCompletionSource() };
        ICommand asCommand = command;

        asCommand.Execute(null); // genuinely void - nothing to await here

        Assert.Equal(1, command.Invocations);
        Assert.True(command.IsExecuting);

        command.Gate!.SetResult();
        await WithTimeout(WaitForIdleAsync(command));

        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute(null));
    }
}
