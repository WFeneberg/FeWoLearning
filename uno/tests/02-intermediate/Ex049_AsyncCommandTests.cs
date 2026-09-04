using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex049_AsyncCommandTests : UnoTestContext
{
    [Fact]
    public void Starts_Out_Executable_And_Idle()
    {
        var command = new Ex049_AsyncCommand(() => Task.CompletedTask);

        Assert.True(command.CanExecute(null));
        Assert.False(command.IsRunning);
        Assert.Null(command.LastError);
    }

    [Fact]
    public void Runs_The_Operation()
    {
        var command = new Ex049_AsyncCommand(() => Task.CompletedTask);

        command.Execute(null);

        Assert.Equal(1, command.Started);
    }

    [Fact]
    public void Reports_Running_While_The_Task_Is_In_Flight()
    {
        var gate = new TaskCompletionSource();
        var command = new Ex049_AsyncCommand(() => gate.Task);

        command.Execute(null);

        Assert.True(command.IsRunning);
        Assert.False(command.CanExecute(null));

        // Settled before the test ends: Execute is async void, and xunit waits for the
        // operations a test posted to its synchronization context. A gate left open here
        // hangs the whole test host, not just this test.
        gate.SetResult();
    }

    [Fact]
    public void Goes_Idle_When_The_Task_Completes()
    {
        var gate = new TaskCompletionSource();
        var command = new Ex049_AsyncCommand(() => gate.Task);
        command.Execute(null);

        gate.SetResult();

        Assert.False(command.IsRunning);
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Refuses_To_Start_A_Second_Run()
    {
        var gate = new TaskCompletionSource();
        var command = new Ex049_AsyncCommand(() => gate.Task);

        command.Execute(null);
        command.Execute(null);

        // Execute is void, so nothing stops a bound button that was pressed twice before
        // the first run finished - the command has to say no itself.
        Assert.Equal(1, command.Started);

        gate.SetResult();
    }

    [Fact]
    public void Announces_Both_Edges()
    {
        var gate = new TaskCompletionSource();
        var command = new Ex049_AsyncCommand(() => gate.Task);
        var notifications = 0;
        command.CanExecuteChanged += (_, _) => notifications++;

        command.Execute(null);
        Assert.Equal(1, notifications);

        gate.SetResult();

        // Announcing only the disable leaves a bound button greyed out forever.
        Assert.Equal(2, notifications);
    }

    [Fact]
    public void Captures_A_Failure_Instead_Of_Crashing()
    {
        var boom = new InvalidOperationException("boom");
        var command = new Ex049_AsyncCommand(() => Task.FromException(boom));

        command.Execute(null);

        // An unhandled exception out of an async void body does not fail a command - it
        // reaches the runtime and takes the process with it.
        Assert.Same(boom, command.LastError);
    }

    [Fact]
    public void Goes_Idle_After_A_Failure()
    {
        var command = new Ex049_AsyncCommand(() => Task.FromException(new InvalidOperationException()));

        command.Execute(null);

        // The flag has to be cleared on the way out of the catch as well, or one failure
        // disables the command permanently.
        Assert.False(command.IsRunning);
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void Can_Run_Again_After_A_Failure()
    {
        var fail = true;
        var command = new Ex049_AsyncCommand(() =>
            fail ? Task.FromException(new InvalidOperationException()) : Task.CompletedTask);

        command.Execute(null);
        fail = false;
        command.Execute(null);

        Assert.Equal(2, command.Started);
    }

    [Fact]
    public void A_Successful_Run_After_A_Failure_Clears_The_Error()
    {
        var fail = true;
        var command = new Ex049_AsyncCommand(() =>
            fail ? Task.FromException(new InvalidOperationException()) : Task.CompletedTask);

        command.Execute(null);
        fail = false;
        command.Execute(null);

        Assert.Null(command.LastError);
    }
}
