using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using FeWoLearning.Avalonia.Exercises.Intermediate;
using FeWoLearning.Avalonia.Tests;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex066_MultiValueConverterTests
{
    private static Ex066_MultiValueConverter Show()
    {
        var view = ViewHarness.Show(new Ex066_MultiValueConverter(), 300, 120);
        Dispatcher.UIThread.RunJobs();
        return view;
    }

    private static Ex066_MultiValueConverterViewModel Vm(Ex066_MultiValueConverter view) =>
        (Ex066_MultiValueConverterViewModel)view.DataContext!;

    private static TextBlock Label(Ex066_MultiValueConverter view) =>
        view.FindControl<TextBlock>("FullName")!;

    // The converter graded on its own, independent of any markup: three values
    // in positional order, one branch each way. This is the half a XAML-only
    // answer cannot fake.
    [AvaloniaFact]
    public void Convert_Composes_The_Three_Values_Positionally()
    {
        var converter = new Ex066_FullNameConverter();

        Assert.Equal(
            "Ada Lovelace",
            converter.Convert(["Ada", "Lovelace", false], typeof(string), null, CultureInfo.InvariantCulture));
        Assert.Equal(
            "Grace Hopper (FRS)",
            converter.Convert(["Grace", "Hopper", true], typeof(string), null, CultureInfo.InvariantCulture));
    }

    // Not defensive padding: a requirement. Measured on this exercise's own view,
    // Avalonia calls the converter once per binding as each one settles, and the
    // FIRST call carries nothing at all -
    //   [UnsetValue, UnsetValue, UnsetValue]
    //   [Ada,        UnsetValue, UnsetValue]
    //   [Ada,        Lovelace,   UnsetValue]
    //   [Ada,        Lovelace,   False]
    // so a Convert that indexes and casts blindly throws before the view has
    // even finished loading.
    [AvaloniaFact]
    public void Convert_Yields_Empty_Text_For_Unset_Or_Wrongly_Typed_Values()
    {
        var converter = new Ex066_FullNameConverter();

        Assert.Equal(
            string.Empty,
            converter.Convert(
                [AvaloniaProperty.UnsetValue, AvaloniaProperty.UnsetValue, AvaloniaProperty.UnsetValue],
                typeof(string), null, CultureInfo.InvariantCulture));
        Assert.Equal(
            string.Empty,
            converter.Convert([], typeof(string), null, CultureInfo.InvariantCulture));
    }

    [AvaloniaFact]
    public void The_View_Renders_The_Composed_Name()
    {
        Assert.Equal("Ada Lovelace", Label(Show()).Text);
    }

    // The discriminator for the markup half. A literal reproduces the resting
    // text above; it cannot follow three separate sources changing. Each
    // property is moved on its own, so a MultiBinding that lists only two of
    // the three fails on whichever one it left out.
    [AvaloniaFact]
    public void The_Text_Follows_Every_One_Of_The_Three_Sources()
    {
        var view = Show();
        var vm = Vm(view);
        var label = Label(view);

        vm.First = "Grace";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Grace Lovelace", label.Text);

        vm.Last = "Hopper";
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Grace Hopper", label.Text);

        vm.IsFellow = true;
        Dispatcher.UIThread.RunJobs();
        Assert.Equal("Grace Hopper (FRS)", label.Text);
    }
}
