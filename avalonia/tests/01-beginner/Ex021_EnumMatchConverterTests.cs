using System.Globalization;
using Avalonia.Data;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Tests.Beginner;

// Plain unit tests against the converter itself, independent of any UI. This is
// the exercise's real subject and the only place that cannot be dodged by
// swapping IsChecked to Mode=OneWay and driving Selected from a Click handler
// instead - ConvertBack in that cheat is never invoked at all, so no UI-level
// test, however cleverly it drives clicks, can force it to run. Testing the
// converter directly closes that gap.
public class Ex021_EnumMatchConverterTests
{
    private static readonly Ex021_EnumMatchConverter Converter = new();

    [Fact]
    public void Convert_Returns_True_Only_When_Value_Equals_Parameter()
    {
        Assert.Equal(true,
            Converter.Convert(Ex021_Choice.Beta, typeof(bool), Ex021_Choice.Beta, CultureInfo.InvariantCulture));
        Assert.Equal(false,
            Converter.Convert(Ex021_Choice.Alpha, typeof(bool), Ex021_Choice.Beta, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertBack_Returns_The_Parameter_When_Checked()
    {
        var result = Converter.ConvertBack(
            true, typeof(Ex021_Choice), Ex021_Choice.Beta, CultureInfo.InvariantCulture);

        Assert.Equal(Ex021_Choice.Beta, result);
    }

    // The exercise's real subject: an unchecking RadioButton also calls
    // ConvertBack with false, and that leg must return BindingOperations.DoNothing
    // specifically - not null, not a default enum member, not anything else - or
    // the unchecking write clobbers whatever another RadioButton in the group
    // just wrote.
    [Fact]
    public void ConvertBack_Returns_DoNothing_When_Unchecked()
    {
        var result = Converter.ConvertBack(
            false, typeof(Ex021_Choice), Ex021_Choice.Beta, CultureInfo.InvariantCulture);

        Assert.Same(BindingOperations.DoNothing, result);
    }
}
