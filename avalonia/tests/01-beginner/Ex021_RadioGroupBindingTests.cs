using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex021_RadioGroupBindingTests
{
    private static (Ex021_RadioGroupBinding View, Ex021_RadioGroupBindingViewModel Vm) Arrange()
    {
        var vm = new Ex021_RadioGroupBindingViewModel();
        var view = ViewHarness.Show(new Ex021_RadioGroupBinding { DataContext = vm }, 300, 120);
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

    [AvaloniaFact]
    public void Renders_The_Vms_Starting_Selection()
    {
        var (view, vm) = Arrange();
        var alpha = view.FindControl<RadioButton>("AlphaRadio")!;
        var beta = view.FindControl<RadioButton>("BetaRadio")!;

        Assert.Equal(Ex021_Choice.Alpha, vm.Selected);
        Assert.Equal(true, alpha.IsChecked);
        Assert.Equal(false, beta.IsChecked);
    }

    // Mechanism check: both radios must share one GroupName, or nothing stops
    // both being checked at once - the thing "grouping" is actually for.
    [AvaloniaFact]
    public void Both_Radios_Share_One_GroupName()
    {
        var (view, _) = Arrange();
        var alpha = view.FindControl<RadioButton>("AlphaRadio")!;
        var beta = view.FindControl<RadioButton>("BetaRadio")!;

        Assert.False(string.IsNullOrEmpty(alpha.GroupName));
        Assert.Equal(alpha.GroupName, beta.GroupName);
    }

    [AvaloniaFact]
    public void Vm_Changes_Flow_To_The_Radios()
    {
        var (view, vm) = Arrange();
        var alpha = view.FindControl<RadioButton>("AlphaRadio")!;
        var beta = view.FindControl<RadioButton>("BetaRadio")!;

        vm.Selected = Ex021_Choice.Beta;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(false, alpha.IsChecked);
        Assert.Equal(true, beta.IsChecked);

        vm.Selected = Ex021_Choice.Alpha;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(true, alpha.IsChecked);
        Assert.Equal(false, beta.IsChecked);
    }

    // The real discriminator: clicking Beta (away from the starting Alpha) also
    // fires ConvertBack(false, AlphaParam) as Alpha unchecks itself. A converter
    // that returns anything but BindingOperations.DoNothing for that false leg -
    // e.g. defaulting to Ex021_Choice.Alpha - lets that unchecking write clobber
    // the Beta selection Alpha's own checking write just made. Clicking back to
    // Alpha the other direction closes the loop with a second, distinct value.
    [AvaloniaFact]
    public void Clicking_A_Radio_Writes_The_Enum_Back_Without_Being_Clobbered()
    {
        var (view, vm) = Arrange();
        var alpha = view.FindControl<RadioButton>("AlphaRadio")!;
        var beta = view.FindControl<RadioButton>("BetaRadio")!;

        Click(beta);
        Assert.Equal(Ex021_Choice.Beta, vm.Selected);
        Assert.Equal(false, alpha.IsChecked);
        Assert.Equal(true, beta.IsChecked);

        Click(alpha);
        Assert.Equal(Ex021_Choice.Alpha, vm.Selected);
        Assert.Equal(true, alpha.IsChecked);
        Assert.Equal(false, beta.IsChecked);
    }
}
