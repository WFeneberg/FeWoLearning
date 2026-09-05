using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex017_ValueConverterTests : WpfTestContext
{
    // Direct, converter-level calls first: these prove the mechanism itself, decoupled
    // from any target-property quirk (e.g. what an unset TextBox.Text reads back as).

    [WpfTheory]
    [InlineData(1, "Low")]
    [InlineData(2, "Medium")]
    [InlineData(3, "High")]
    public void Convert_Maps_Known_Codes_To_Their_Label(int code, string expectedLabel)
    {
        var converter = new Ex017_PriorityConverter();

        var result = converter.Convert(code, typeof(string), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expectedLabel, result);
    }

    [WpfFact]
    public void Convert_Returns_UnsetValue_For_An_Unknown_Code()
    {
        var converter = new Ex017_PriorityConverter();

        var result = converter.Convert(99, typeof(string), null!, CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, result);
    }

    [WpfFact]
    public void Convert_Returns_UnsetValue_For_A_Non_Int_Input()
    {
        var converter = new Ex017_PriorityConverter();

        var result = converter.Convert("not a code", typeof(string), null!, CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, result);
    }

    [WpfTheory]
    [InlineData("Low", 1)]
    [InlineData("Medium", 2)]
    [InlineData("High", 3)]
    public void ConvertBack_Maps_Known_Labels_To_Their_Code(string label, int expectedCode)
    {
        var converter = new Ex017_PriorityConverter();

        var result = converter.ConvertBack(label, typeof(int), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expectedCode, result);
    }

    [WpfFact]
    public void ConvertBack_Returns_UnsetValue_For_An_Unknown_Label()
    {
        var converter = new Ex017_PriorityConverter();

        var result = converter.ConvertBack("Nonsense", typeof(int), null!, CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, result);
    }

    // Now through a live, two-way binding, proving Bind() actually wires the converter
    // up rather than only the converter class working in isolation.

    [WpfFact]
    public void The_Display_Shows_The_Label_For_The_Sources_Initial_Code()
    {
        var source = new Ex017_PrioritySource { PriorityCode = 2 };
        var target = new TextBox();

        Ex017_ValueConverter.Bind(target, source);
        Layout(target);
        Pump();

        Assert.Equal("Medium", target.Text);
    }

    [WpfFact]
    public void A_Later_Code_Change_On_The_Source_Still_Reaches_The_Target()
    {
        var source = new Ex017_PrioritySource { PriorityCode = 1 };
        var target = new TextBox();
        Ex017_ValueConverter.Bind(target, source);
        Layout(target);
        Pump();

        source.PriorityCode = 3;
        Pump();

        Assert.Equal("High", target.Text);
    }

    [WpfFact]
    public void Editing_The_Target_To_A_Known_Label_Pushes_The_Code_Back_To_The_Source()
    {
        var source = new Ex017_PrioritySource { PriorityCode = 1 };
        var target = new TextBox();
        Ex017_ValueConverter.Bind(target, source);
        Layout(target);
        Pump();

        target.Text = "High";
        Pump();

        Assert.Equal(3, source.PriorityCode);
    }

    [WpfFact]
    public void Editing_The_Target_To_An_Unknown_Label_Does_Not_Push_Anything_To_The_Source()
    {
        var source = new Ex017_PrioritySource { PriorityCode = 1 };
        var target = new TextBox();
        Ex017_ValueConverter.Bind(target, source);
        Layout(target);
        Pump();

        target.Text = "Nonsense";
        Pump();

        // UnsetValue on the way back means "do not push" - the source keeps whatever
        // it had, it is not reset to 0 or thrown into an inconsistent state.
        Assert.Equal(1, source.PriorityCode);
    }

    [WpfFact]
    public void The_Binding_Is_Declared_TwoWay_Through_The_Priority_Converter()
    {
        var source = new Ex017_PrioritySource();
        var target = new TextBox();

        Ex017_ValueConverter.Bind(target, source);

        var binding = BindingOperations.GetBinding(target, TextBox.TextProperty);

        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex017_PrioritySource.PriorityCode), binding!.Path.Path);
        Assert.Equal(BindingMode.TwoWay, binding.Mode);
        Assert.IsType<Ex017_PriorityConverter>(binding.Converter);
    }
}
