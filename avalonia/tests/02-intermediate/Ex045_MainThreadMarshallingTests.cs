using System.Windows.Input;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex045_MainThreadMarshallingTests
{
    // Runs FetchCommand.Execute() on a background thread and bounds the wait, exactly
    // as Ex039_CommandFromTaskTests.ExecuteWithoutBlocking does: a solution that uses
    // ReactiveCommand.Create(() => _work().GetAwaiter().GetResult()) instead of
    // CreateFromTask would deadlock the calling thread inside Execute() itself, and a
    // test calling that directly would hang dotnet test rather than fail it.
    private static Task<string> ExecuteWithoutBlocking(Ex045_MainThreadMarshallingViewModel vm)
    {
        Task<string>? running = null;
        var executeReturned = Task.Run(() => { running = vm.FetchCommand.Execute().ToTask(); });

        if (!executeReturned.Wait(TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException(
                "FetchCommand.Execute() did not return within 5s - it appears to block the calling " +
                "thread synchronously instead of genuinely awaiting the async work.");
        }

        return running!;
    }

    // The discriminator this whole exercise is about: the background work finishing
    // must NOT be enough to update Result. Only draining the dispatcher does. This is
    // section 7 of the track design doc, made explicit: "anything scheduled through
    // the main-thread scheduler has not run yet when the assertion executes."
    [AvaloniaFact]
    public void Result_Is_Not_Touched_Until_The_Dispatcher_Drains_The_Marshalled_Job()
    {
        var vm = new Ex045_MainThreadMarshallingViewModel(() => Task.Run(() => "async-value"));

        var running = ExecuteWithoutBlocking(vm);
        Assert.True(running.Wait(TimeSpan.FromSeconds(5)), "the background work did not complete within 5s");

        // A small real-time margin, deliberately: a cheat that updates Result from a
        // stray, uncoordinated background continuation (rather than genuinely through
        // the dispatcher) could otherwise land inside the race window between the
        // background task completing and this assertion running.
        Thread.Sleep(50);

        Assert.Equal(string.Empty, vm.Result);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("async-value", vm.Result);
    }

    // A second, distinctly-valued run: guards against a solution that hard-codes one
    // string instead of genuinely relaying the awaited result.
    [AvaloniaFact]
    public void A_Second_Distinctly_Valued_Run_Also_Marshals_Correctly()
    {
        var vm = new Ex045_MainThreadMarshallingViewModel(() => Task.Run(() => "a completely different value"));

        var running = ExecuteWithoutBlocking(vm);
        Assert.True(running.Wait(TimeSpan.FromSeconds(5)), "the background work did not complete within 5s");
        Thread.Sleep(50);
        Assert.Equal(string.Empty, vm.Result);

        Dispatcher.UIThread.RunJobs();

        Assert.Equal("a completely different value", vm.Result);
    }

    // Structural check via ICommand: FetchCommand must be a real, executable command -
    // not merely something that happens to make the behavioural assertions above pass.
    [AvaloniaFact]
    public void FetchCommand_Is_A_Real_Executable_Command()
    {
        var vm = new Ex045_MainThreadMarshallingViewModel(() => Task.Run(() => "x"));

        Assert.True(((ICommand)vm.FetchCommand).CanExecute(null));
    }
}
