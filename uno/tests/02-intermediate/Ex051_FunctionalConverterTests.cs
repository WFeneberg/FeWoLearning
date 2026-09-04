using FeWoLearning.Uno.Exercises.Intermediate;
using Microsoft.UI.Xaml;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex051_FunctionalConverterTests : UnoTestContext
{
    [Fact]
    public void Applies_The_Forward_Delegate()
    {
        var converter = Ex051_Convert.OneWay<int, string>(value => $"#{value}");

        Assert.Equal("#7", converter.Convert(7, typeof(string), null!, ""));
    }

    [Fact]
    public void A_One_Way_Converter_Has_No_Answer_Backwards()
    {
        var converter = Ex051_Convert.OneWay<int, string>(value => $"#{value}");

        // Not an exception and not a guess: UnsetValue lets the binding fall back.
        Assert.Equal(DependencyProperty.UnsetValue, converter.ConvertBack("#7", typeof(int), null!, ""));
    }

    [Fact]
    public void A_Two_Way_Converter_Answers_Both_Ways()
    {
        var converter = Ex051_Convert.TwoWay<int, string>(value => $"#{value}", text => int.Parse(text[1..]));

        Assert.Equal("#7", converter.Convert(7, typeof(string), null!, ""));
        Assert.Equal(7, converter.ConvertBack("#7", typeof(int), null!, ""));
    }

    [Fact]
    public void Input_Of_The_Wrong_Type_Is_Left_Unset()
    {
        var converter = Ex051_Convert.OneWay<int, string>(value => $"#{value}");

        // A binding can see the wrong type for one pass while a source is swapped - the
        // converter must not be the thing that crashes.
        Assert.Equal(DependencyProperty.UnsetValue, converter.Convert("not an int", typeof(string), null!, ""));
    }

    [Fact]
    public void Null_Is_Left_Unset_For_A_Value_Type()
    {
        var converter = Ex051_Convert.OneWay<int, string>(value => $"#{value}");

        Assert.Equal(DependencyProperty.UnsetValue, converter.Convert(null!, typeof(string), null!, ""));
    }

    [Fact]
    public void A_Reference_Type_Accepts_Null()
    {
        var converter = Ex051_Convert.OneWay<string?, string>(text => text ?? "(none)");

        Assert.Equal("(none)", converter.Convert(null!, typeof(string), null!, ""));
    }

    [Fact]
    public void Wrong_Type_Backwards_Is_Also_Unset()
    {
        var converter = Ex051_Convert.TwoWay<int, string>(value => $"#{value}", text => int.Parse(text[1..]));

        Assert.Equal(DependencyProperty.UnsetValue, converter.ConvertBack(42, typeof(int), null!, ""));
    }

    [Fact]
    public void The_Delegate_Decides_Everything()
    {
        var upper = Ex051_Convert.OneWay<string, string>(text => text.ToUpperInvariant());
        var length = Ex051_Convert.OneWay<string, int>(text => text.Length);

        // Two converters, no new classes - which is the point of the exercise.
        Assert.Equal("ABC", upper.Convert("abc", typeof(string), null!, ""));
        Assert.Equal(3, length.Convert("abc", typeof(int), null!, ""));
    }
}
