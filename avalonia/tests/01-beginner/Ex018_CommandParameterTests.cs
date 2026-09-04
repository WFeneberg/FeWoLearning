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

public class Ex018_CommandParameterTests
{
    private static (Ex018_CommandParameter View, Ex018_CommandParameterViewModel Vm) Arrange()
    {
        var vm = new Ex018_CommandParameterViewModel();
        var view = ViewHarness.Show(new Ex018_CommandParameter { DataContext = vm }, 300, 160);
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

    // Proves the command itself forwards whatever parameter it is given,
    // independent of any button - a command that ignores its parameter and
    // hard-codes the result would fail this for at least one of two values.
    [AvaloniaFact]
    public void Invoking_The_Command_Directly_Stores_Whatever_Parameter_It_Is_Given()
    {
        var (_, vm) = Arrange();

        ((ICommand)vm.SetParameterCommand).Execute("gamma");
        Assert.Equal("gamma", vm.LastParameter);

        ((ICommand)vm.SetParameterCommand).Execute("delta");
        Assert.Equal("delta", vm.LastParameter);
    }

    // Structural discriminator: both buttons must route through the SAME
    // command instance - that is the exercise's whole point, one parameterised
    // command serving two call sites. Without this, a pair of Click handlers
    // that set vm.LastParameter directly (never touching SetParameterCommand
    // at all) would satisfy every other assertion in this file.
    [AvaloniaFact]
    public void Each_Button_Declares_Its_Own_CommandParameter_And_Is_Bound_To_The_Shared_Command()
    {
        var (view, vm) = Arrange();
        var alpha = view.FindControl<Button>("AlphaButton")!;
        var beta = view.FindControl<Button>("BetaButton")!;

        Assert.Equal("alpha", alpha.CommandParameter);
        Assert.Equal("beta", beta.CommandParameter);
        Assert.Same(vm.SetParameterCommand, alpha.Command);
        Assert.Same(vm.SetParameterCommand, beta.Command);
    }

    // Clicking BETA before ALPHA on purpose: a solution that hard-codes "alpha"
    // as the expected/returned value, or that only wires the first button it
    // finds, fails this ordering.
    [AvaloniaFact]
    public void Clicking_Each_Button_Sends_Its_Own_Parameter_Through_The_Bound_Command()
    {
        var (view, vm) = Arrange();
        var alpha = view.FindControl<Button>("AlphaButton")!;
        var beta = view.FindControl<Button>("BetaButton")!;

        Click(beta);
        Assert.Equal("beta", vm.LastParameter);

        Click(alpha);
        Assert.Equal("alpha", vm.LastParameter);
    }
}
