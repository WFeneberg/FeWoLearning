using ReactiveUI.Primitives;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex041_CommandThrownExceptionsTests
{
    // The two-channel discriminator. A solution that swallows the failure inside its
    // own wrapper (try/catch around _work(), manually setting LastError in the catch,
    // then returning a fallback string) reproduces the LastError assertion below but
    // makes the command "succeed" - Execute().ToTask() would complete normally instead
    // of throwing. Both assertions together are what only a genuine
    // FetchCommand.ThrownExceptions subscription satisfies: the underlying task must
    // still genuinely fault (nothing swallowed it), AND the failure must have been
    // surfaced onto the view model (nothing left unobserved).
    [Fact]
    public async Task A_Failing_Task_Both_Faults_The_Execution_And_Surfaces_On_LastError()
    {
        var vm = new Ex041_CommandThrownExceptionsViewModel(
            () => Task.FromException<string>(new InvalidOperationException("boom")));

        var thrown = await Record.ExceptionAsync(
            () => vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken));

        Assert.NotNull(thrown);
        Assert.IsType<InvalidOperationException>(thrown);
        Assert.Equal("boom", thrown.Message);
        Assert.Equal("boom", vm.LastError);
    }

    // A second, differently-valued failure on a fresh view model: guards against a
    // solution that hard-codes one message instead of genuinely relaying ex.Message.
    [Fact]
    public async Task A_Second_Distinctly_Messaged_Failure_Also_Surfaces_Correctly()
    {
        var vm = new Ex041_CommandThrownExceptionsViewModel(
            () => Task.FromException<string>(new InvalidOperationException("a totally different failure")));

        var thrown = await Record.ExceptionAsync(
            () => vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken));

        Assert.NotNull(thrown);
        Assert.Equal("a totally different failure", thrown.Message);
        Assert.Equal("a totally different failure", vm.LastError);
    }

    // Structural check, in the spirit of the "cheat must live where the learner
    // writes" rule: nothing about LastError being set proves it came from
    // ThrownExceptions specifically rather than a manual catch inside the wrapper -
    // this is exactly what the fault-propagation assertion above rules out, since a
    // manual catch necessarily prevents the underlying task from faulting at all.
    // A successful run must never touch LastError.
    [Fact]
    public async Task A_Successful_Run_Leaves_LastError_Untouched()
    {
        var vm = new Ex041_CommandThrownExceptionsViewModel(() => Task.FromResult("ok"));

        var result = await vm.FetchCommand.Execute().ToTask(TestContext.Current.CancellationToken);

        Assert.Equal("ok", result);
        Assert.Null(vm.LastError);
    }
}
