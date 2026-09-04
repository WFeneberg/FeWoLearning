using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex011_BindingModesTests
{
    private static (Ex011_BindingModes View, Ex011_BindingModesViewModel Vm) Arrange()
    {
        var vm = new Ex011_BindingModesViewModel();
        var view = ViewHarness.Show(new Ex011_BindingModes { DataContext = vm }, 300, 160);
        return (view, vm);
    }

    [AvaloniaFact]
    public void OneWay_Renders_The_Vm_Value_But_Typing_Does_Not_Write_Back()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<TextBox>("OneWayBox")!;

        Assert.Equal("one-way-initial", box.Text);

        box.Text = "typed";
        Dispatcher.UIThread.RunJobs();

        Assert.Equal("one-way-initial", vm.OneWayValue);
    }

    [AvaloniaFact]
    public void TwoWay_Renders_And_Round_Trips_Both_Directions()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<TextBox>("TwoWayBox")!;

        Assert.Equal("two-way-initial", box.Text);

        box.Text = "typed-into-box";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("typed-into-box", vm.TwoWayValue);

        vm.TwoWayValue = "set-from-vm";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("set-from-vm", box.Text);
    }

    // The real discriminator between the three modes: OneWayToSource clobbers the
    // VM's starting value with the target's own (empty) value the instant the
    // binding applies, before any typing happens. A TwoWay binding on the same
    // property would instead show and keep "to-source-initial" at load, and a
    // OneWay binding would push it in without ever clobbering it. This is the only
    // reliable way to tell OneWayToSource apart from the other two in a test.
    [AvaloniaFact]
    public void OneWayToSource_Clobbers_The_Vm_At_Load_Then_Only_Pulls_From_The_Target()
    {
        var vm = new Ex011_BindingModesViewModel();
        Assert.Equal("to-source-initial", vm.ToSourceValue);

        var view = ViewHarness.Show(new Ex011_BindingModes { DataContext = vm }, 300, 160);
        Dispatcher.UIThread.RunJobs();

        Assert.Null(vm.ToSourceValue);

        var box = view.FindControl<TextBox>("ToSourceBox")!;
        box.Text = "typed-to-source";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("typed-to-source", vm.ToSourceValue);

        // And the VM never reaches the target: a later VM change does not show up.
        vm.ToSourceValue = "set-from-vm-again";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("typed-to-source", box.Text);
    }
}
