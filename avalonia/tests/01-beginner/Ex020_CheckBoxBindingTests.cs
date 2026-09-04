using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex020_CheckBoxBindingTests
{
    private static (Ex020_CheckBoxBinding View, Ex020_CheckBoxBindingViewModel Vm) Arrange()
    {
        var vm = new Ex020_CheckBoxBindingViewModel();
        var view = ViewHarness.Show(new Ex020_CheckBoxBinding { DataContext = vm }, 300, 120);
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
    public void Renders_The_Vms_Starting_Null_State()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<CheckBox>("ThreeStateBox")!;

        Assert.Null(vm.IsChecked);
        Assert.Null(box.IsChecked);
    }

    // The discriminator: a plain two-state CheckBox (IsThreeState missing or
    // false) can never land on null again - it would cycle False/True/False.
    // Three clicks from the null start must retrace exactly this sequence.
    [AvaloniaFact]
    public void Clicking_Cycles_Null_To_False_To_True_To_Null()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<CheckBox>("ThreeStateBox")!;

        Click(box);
        Assert.Equal(false, vm.IsChecked);
        Assert.Equal(false, box.IsChecked);

        Click(box);
        Assert.Equal(true, vm.IsChecked);
        Assert.Equal(true, box.IsChecked);

        Click(box);
        Assert.Null(vm.IsChecked);
        Assert.Null(box.IsChecked);
    }

    [AvaloniaFact]
    public void Vm_Changes_Flow_To_The_Box_Through_All_Three_States()
    {
        var (view, vm) = Arrange();
        var box = view.FindControl<CheckBox>("ThreeStateBox")!;

        vm.IsChecked = true;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(true, box.IsChecked);

        vm.IsChecked = false;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(false, box.IsChecked);

        vm.IsChecked = null;
        Dispatcher.UIThread.RunJobs();
        Assert.Null(box.IsChecked);
    }
}
