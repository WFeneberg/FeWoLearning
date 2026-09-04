using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Beginner;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Beginner;

public class Ex015_ValueConverterTests
{
    private static (Ex015_ValueConverter View, Ex015_ValueConverterViewModel Vm) Arrange(double celsius)
    {
        var vm = new Ex015_ValueConverterViewModel { Celsius = celsius };
        var view = ViewHarness.Show(new Ex015_ValueConverter { DataContext = vm }, 300, 100);
        return (view, vm);
    }

    [AvaloniaFact]
    public void Convert_Renders_Fahrenheit_From_The_Vms_Celsius()
    {
        var (view, _) = Arrange(celsius: 100);
        var box = view.FindControl<TextBox>("FahrenheitBox")!;

        Assert.Equal("212", box.Text);
    }

    // The real discriminator: a converter that implements only Convert (and
    // throws, or no-ops, in ConvertBack) renders the display above just fine but
    // must fail here. Binding is TwoWay, so typing into the box has to round-trip
    // back through ConvertBack into Celsius - not just display correctly.
    [AvaloniaFact]
    public void ConvertBack_Writes_Celsius_From_Typed_Fahrenheit()
    {
        var (view, vm) = Arrange(celsius: 0);
        var box = view.FindControl<TextBox>("FahrenheitBox")!;

        box.Text = "32";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(0, vm.Celsius);

        box.Text = "212";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(100, vm.Celsius);
    }
}
