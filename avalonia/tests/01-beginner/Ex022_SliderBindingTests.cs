using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex022_SliderBindingTests
{
    private static (Ex022_SliderBinding View, Ex022_SliderBindingViewModel Vm) Arrange()
    {
        var vm = new Ex022_SliderBindingViewModel();
        var view = ViewHarness.Show(new Ex022_SliderBinding { DataContext = vm }, 300, 100);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Renders_The_Vms_Starting_Value_Inside_The_Configured_Range()
    {
        var (view, _) = Arrange();
        var slider = view.FindControl<Slider>("ValueSlider")!;

        Assert.Equal(10, slider.Minimum);
        Assert.Equal(20, slider.Maximum);
        Assert.Equal(15, slider.Value);
    }

    // Guards against a hard-coded Value="20": pushing the view model to a
    // second, in-range value the slider never started at or would coincide
    // with must still show up on the slider.
    [AvaloniaFact]
    public void Vm_Change_Within_Range_Flows_To_The_Slider()
    {
        var (view, vm) = Arrange();
        var slider = view.FindControl<Slider>("ValueSlider")!;

        vm.Value = 17;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(17, slider.Value);
    }

    // The real subject: a Slider clamps an out-of-range assignment, and
    // because the binding is TwoWay, the clamped value writes back into the
    // view model too - not just the slider. Both extremes, both sides.
    [AvaloniaFact]
    public void Vm_Push_Out_Of_Range_Clamps_The_Slider_And_Writes_The_Clamp_Back()
    {
        var (view, vm) = Arrange();
        var slider = view.FindControl<Slider>("ValueSlider")!;

        vm.Value = 99;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(20, slider.Value);
        Assert.Equal(20, vm.Value);

        vm.Value = -5;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(10, slider.Value);
        Assert.Equal(10, vm.Value);
    }

    // UI-side leg of the same TwoWay binding: a value written directly onto
    // the slider (standing in for a drag) must reach the view model too.
    [AvaloniaFact]
    public void Slider_Side_Change_Writes_Back_To_The_Vm()
    {
        var (view, vm) = Arrange();
        var slider = view.FindControl<Slider>("ValueSlider")!;

        slider.Value = 12;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(12, vm.Value);
    }
}
