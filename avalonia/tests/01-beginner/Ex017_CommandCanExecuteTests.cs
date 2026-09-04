using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex017_CommandCanExecuteTests
{
    private static (Ex017_CommandCanExecute View, Ex017_CommandCanExecuteViewModel Vm) Arrange()
    {
        var vm = new Ex017_CommandCanExecuteViewModel();
        var view = ViewHarness.Show(new Ex017_CommandCanExecute { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    private static void Click(Visual target)
    {
        var top = TopLevel.GetTopLevel(target)!;
        var p = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), top)!.Value;
        top.MouseDown(p, MouseButton.Left);
        top.MouseUp(p, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    // THE BIG TRAP this exercise is about: Button.IsEnabled is the property the
    // learner sets (or, here, never touches) - it does NOT reflect CanExecute.
    // IsEffectivelyEnabled and the ":disabled" pseudo-class do. Asserting
    // IsEnabled == false here would fail against a correct solution.
    [AvaloniaFact]
    public void Button_Is_Wired_To_The_Vms_Command_And_Effectively_Enabled_By_Default()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("RunButton")!;

        Assert.Same(vm.RunCommand, button.Command);
        Assert.True(button.IsEffectivelyEnabled);
        Assert.True(((ICommand)vm.RunCommand).CanExecute(null));
        Assert.DoesNotContain(":disabled", button.Classes);
    }

    // A hard-coded IsEnabled="False" (or a command with no canExecute wired at
    // all) must fail here: the state has to move in BOTH directions as CanRun
    // changes, not just start or land on one fixed value.
    [AvaloniaFact]
    public void Disabling_And_Reenabling_CanRun_Flips_Effective_State_In_Both_Directions()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("RunButton")!;

        vm.CanRun = false;
        Dispatcher.UIThread.RunJobs();

        Assert.False(button.IsEffectivelyEnabled);
        Assert.False(((ICommand)vm.RunCommand).CanExecute(null));
        Assert.Contains(":disabled", button.Classes);

        vm.CanRun = true;
        Dispatcher.UIThread.RunJobs();

        Assert.True(button.IsEffectivelyEnabled);
        Assert.True(((ICommand)vm.RunCommand).CanExecute(null));
        Assert.DoesNotContain(":disabled", button.Classes);
    }

    [AvaloniaFact]
    public void Clicking_A_Disabled_Button_Does_Nothing_But_An_Enabled_Click_Runs_The_Command()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("RunButton")!;

        vm.CanRun = false;
        Dispatcher.UIThread.RunJobs();

        Click(button);
        Assert.Equal(0, vm.Count);

        vm.CanRun = true;
        Dispatcher.UIThread.RunJobs();

        Click(button);
        Assert.Equal(1, vm.Count);
    }
}
