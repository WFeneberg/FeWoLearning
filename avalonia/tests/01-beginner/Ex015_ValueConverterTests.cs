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

    // Two distinct Celsius values must render two distinct Fahrenheit strings -
    // a Convert hard-coded to a single (Celsius, Fahrenheit) pair, or one that
    // ignores its input, fails here.
    [AvaloniaFact]
    public void Convert_Renders_Fahrenheit_From_The_Vms_Celsius()
    {
        var (viewA, _) = Arrange(celsius: 20);
        Assert.Equal("68", viewA.FindControl<TextBox>("FahrenheitBox")!.Text);

        var (viewB, _) = Arrange(celsius: 100);
        Assert.Equal("212", viewB.FindControl<TextBox>("FahrenheitBox")!.Text);
    }

    // The real discriminator: a converter that implements only Convert (and
    // throws, or no-ops, in ConvertBack) renders the display above just fine but
    // must fail here. Binding is TwoWay, so typing into the box has to round-trip
    // back through ConvertBack into Celsius - not just display correctly.
    //
    // Two things this seeding avoids, both real defects found by review:
    //   1. Seeding from Celsius 0 renders "32" - typing "32" back in is then a
    //      no-op (new value equals the value the box already holds), and
    //      Avalonia's property system skips the change notification entirely,
    //      so ConvertBack is never even called. Seeding from 20 (renders "68")
    //      means both "50" and "212" below are genuine changes.
    //   2. Asserting only Celsius 100 from typed "212" is a tautology: feeding
    //      Convert's own output back into ConvertBack can never distinguish a
    //      real inverse from a converter hard-coded to that one literal pair.
    //      Typing "50" first - a value never rendered anywhere in this test -
    //      and asserting a DIFFERENT, non-hard-codeable Celsius result closes
    //      that gap.
    [AvaloniaFact]
    public void ConvertBack_Writes_Celsius_From_Typed_Fahrenheit()
    {
        var (view, vm) = Arrange(celsius: 20);
        var box = view.FindControl<TextBox>("FahrenheitBox")!;
        Assert.Equal("68", box.Text);

        box.Text = "50";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(10.0, vm.Celsius, 0.0001);

        box.Text = "212";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(100.0, vm.Celsius, 0.0001);
    }
}
