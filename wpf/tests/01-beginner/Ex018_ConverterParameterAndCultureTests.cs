using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex018_ConverterParameterAndCultureTests : WpfTestContext
{
    private static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo Germany = CultureInfo.GetCultureInfo("de-DE");

    // Direct converter calls, decoupled from any target-property quirk.

    [WpfFact]
    public void Convert_Multiplies_By_The_ConverterParameter_Quantity()
    {
        var converter = new Ex018_TotalPriceConverter();

        var result = converter.Convert(10m, typeof(string), 3, UnitedStates);

        Assert.Equal((10m * 3).ToString("C", UnitedStates), result);
    }

    [WpfFact]
    public void Convert_Uses_A_Different_Quantity_When_The_Parameter_Differs()
    {
        var converter = new Ex018_TotalPriceConverter();

        // Only the parameter changes from the test above - a learner who hard-coded
        // the multiplier (e.g. always 1, or always 3) fails one of the two.
        var result = converter.Convert(10m, typeof(string), 5, UnitedStates);

        Assert.Equal((10m * 5).ToString("C", UnitedStates), result);
    }

    [WpfFact]
    public void Convert_Formats_In_The_Given_Culture_Not_A_Fixed_One()
    {
        var converter = new Ex018_TotalPriceConverter();

        var us = converter.Convert(10m, typeof(string), 3, UnitedStates);
        var de = converter.Convert(10m, typeof(string), 3, Germany);

        Assert.Equal((30m).ToString("C", UnitedStates), us);
        Assert.Equal((30m).ToString("C", Germany), de);
        // These two cultures format the same total differently (en-US: "$30.00",
        // de-DE: "30,00 €") - so a learner who ignored `culture` and always used
        // CultureInfo.InvariantCulture (or CurrentCulture) would produce the same
        // string for both calls instead.
        Assert.NotEqual(us, de);
    }

    // Now through a live binding, proving Bind() actually wires ConverterParameter and
    // ConverterCulture rather than only the converter class working in isolation.

    [WpfFact]
    public void The_Display_Reflects_Quantity_And_Culture_Through_A_Live_Binding()
    {
        var source = new Ex018_LineItemSource { UnitPrice = 10m };
        var target = new TextBlock();

        Ex018_ConverterParameterAndCulture.Bind(target, source, quantity: 3, UnitedStates);
        Layout(target);
        Pump();

        Assert.Equal((30m).ToString("C", UnitedStates), target.Text);
    }

    [WpfFact]
    public void The_Display_Uses_The_German_Culture_When_Bound_With_It()
    {
        var source = new Ex018_LineItemSource { UnitPrice = 10m };
        var target = new TextBlock();

        Ex018_ConverterParameterAndCulture.Bind(target, source, quantity: 3, Germany);
        Layout(target);
        Pump();

        Assert.Equal((30m).ToString("C", Germany), target.Text);
    }

    [WpfFact]
    public void A_Later_UnitPrice_Change_Still_Reaches_The_Target()
    {
        var source = new Ex018_LineItemSource { UnitPrice = 10m };
        var target = new TextBlock();
        Ex018_ConverterParameterAndCulture.Bind(target, source, quantity: 2, UnitedStates);
        Layout(target);
        Pump();

        source.UnitPrice = 25m;
        Pump();

        Assert.Equal((50m).ToString("C", UnitedStates), target.Text);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_With_The_Given_Parameter_And_Culture()
    {
        var source = new Ex018_LineItemSource();
        var target = new TextBlock();

        Ex018_ConverterParameterAndCulture.Bind(target, source, quantity: 7, Germany);

        var binding = BindingOperations.GetBinding(target, TextBlock.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex018_LineItemSource.UnitPrice), binding!.Path.Path);
        Assert.IsType<Ex018_TotalPriceConverter>(binding.Converter);
        Assert.Equal(7, binding.ConverterParameter);
        Assert.Equal(Germany, binding.ConverterCulture);
    }
}
