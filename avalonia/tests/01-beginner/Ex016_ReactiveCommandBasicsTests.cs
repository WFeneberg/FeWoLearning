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

public class Ex016_ReactiveCommandBasicsTests
{
    private static (Ex016_ReactiveCommandBasics View, Ex016_ReactiveCommandBasicsViewModel Vm) Arrange()
    {
        var vm = new Ex016_ReactiveCommandBasicsViewModel();
        var view = ViewHarness.Show(new Ex016_ReactiveCommandBasics { DataContext = vm }, 300, 120);
        return (view, vm);
    }

    // A real headless click through the actual input pipeline - not a synthetic
    // "invoke the handler" call. Needs Avalonia.Headless for the mouse extensions
    // and Avalonia.Input for MouseButton.
    private static void Click(Visual target)
    {
        var top = TopLevel.GetTopLevel(target)!;
        var p = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), top)!.Value;
        top.MouseDown(p, MouseButton.Left);
        top.MouseUp(p, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    // Discriminator: a code-behind Click handler that increments Counter directly
    // would satisfy a naive "count went up" test without any ReactiveCommand ever
    // existing. Assert the button is actually wired to the vm's command instance.
    [AvaloniaFact]
    public void Button_Is_Wired_To_The_Vms_Command()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("IncrementButton")!;

        Assert.Same(vm.IncrementCommand, button.Command);
    }

    [AvaloniaFact]
    public void Invoking_The_Command_Directly_Increments_Counter()
    {
        var (_, vm) = Arrange();

        ((ICommand)vm.IncrementCommand).Execute(null);
        Assert.Equal(1, vm.Counter);

        ((ICommand)vm.IncrementCommand).Execute(null);
        Assert.Equal(2, vm.Counter);
    }

    [AvaloniaFact]
    public void Clicking_The_Button_Invokes_The_Command_And_Updates_The_View()
    {
        var (view, vm) = Arrange();
        var button = view.FindControl<Button>("IncrementButton")!;
        var counterText = view.FindControl<TextBlock>("CounterText")!;

        Click(button);
        Assert.Equal(1, vm.Counter);
        Assert.Equal("1", counterText.Text);

        Click(button);
        Assert.Equal(2, vm.Counter);
        Assert.Equal("2", counterText.Text);
    }
}
