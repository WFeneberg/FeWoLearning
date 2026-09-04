using ReactiveUI.Primitives;
using System.Windows.Input;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex040_CommandIsExecutingTests
{
    [Fact]
    public void Starts_Idle_And_Executable()
    {
        var vm = new Ex040_CommandIsExecutingViewModel(() => Task.FromResult("x"));

        Assert.False(vm.IsBusy);
        Assert.True(((ICommand)vm.RunCommand).CanExecute(null));
    }

    // The deterministic gate pattern, and the exercise's whole point: this fails
    // outright against ReactiveCommand.CreateFromTask(_work) with no sequencer -
    // that overload's IsExecuting never reports true, so IsBusy would still read
    // false here and this assertion would catch it.
    [Fact]
    public async Task IsBusy_Goes_True_Mid_Flight_And_CanExecute_Is_Gated_False()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex040_CommandIsExecutingViewModel(() => gate.Task);

        var running = vm.RunCommand.Execute().ToTask(TestContext.Current.CancellationToken);

        Assert.True(vm.IsBusy);
        Assert.False(((ICommand)vm.RunCommand).CanExecute(null));

        gate.SetResult("done");
        await running;

        Assert.False(vm.IsBusy);
        Assert.True(((ICommand)vm.RunCommand).CanExecute(null));
    }

    [Fact]
    public async Task IsBusy_Raises_PropertyChanged_True_Then_False_In_Order()
    {
        var gate = new TaskCompletionSource<string>();
        var vm = new Ex040_CommandIsExecutingViewModel(() => gate.Task);
        var raised = new List<bool>();
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Ex040_CommandIsExecutingViewModel.IsBusy))
            {
                raised.Add(vm.IsBusy);
            }
        };

        var running = vm.RunCommand.Execute().ToTask(TestContext.Current.CancellationToken);
        gate.SetResult("done");
        await running;

        Assert.Equal([true, false], raised);
    }

    // A second run on the SAME view model re-arms the gate too - guards against
    // a wiring built from a one-shot observable that only reports busy once.
    [Fact]
    public async Task A_Second_Run_On_The_Same_View_Model_Also_Gates_Correctly()
    {
        TaskCompletionSource<string>? currentGate = null;
        var vm = new Ex040_CommandIsExecutingViewModel(() =>
        {
            currentGate = new TaskCompletionSource<string>();
            return currentGate.Task;
        });

        var firstRun = vm.RunCommand.Execute().ToTask(TestContext.Current.CancellationToken);
        Assert.True(vm.IsBusy);
        currentGate!.SetResult("first");
        await firstRun;
        Assert.False(vm.IsBusy);

        var secondRun = vm.RunCommand.Execute().ToTask(TestContext.Current.CancellationToken);
        Assert.True(vm.IsBusy);
        currentGate!.SetResult("second");
        await secondRun;
        Assert.False(vm.IsBusy);
    }
}
