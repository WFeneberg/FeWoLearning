using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex007_ValueConverterTests : UnoTestContext
{
    private static readonly IValueConverter Converter = new Ex007_ValueConverter();

    [Theory]
    [InlineData("en-US", "1,234.50")]
    [InlineData("de-DE", "1.234,50")]
    // Deliberately no fr-FR: its group separator has been a normal space, a no-break
    // space and a narrow no-break space across ICU versions. Not a lesson about Uno.
    public void Formats_With_The_Separators_Of_The_Language(string language, string expected)
    {
        Assert.Equal(expected, Converter.Convert(1234.5d, typeof(string), null!, language));
    }

    [Fact]
    public void An_Empty_Language_Means_Invariant_Culture()
    {
        // The binding engine passes "" when nothing on the element set a Language.
        Assert.Equal("1,234.50", Converter.Convert(1234.5d, typeof(string), null!, ""));
    }

    [Fact]
    public void Always_Shows_Two_Decimals()
    {
        Assert.Equal("7.00", Converter.Convert(7d, typeof(string), null!, ""));
    }

    [Fact]
    public void Input_It_Cannot_Handle_Is_Left_Unset()
    {
        // Returning UnsetValue lets FallbackValue take over; returning null or a message
        // would put "null" or the message on screen.
        Assert.Equal(DependencyProperty.UnsetValue, Converter.Convert("not a number", typeof(string), null!, ""));
        Assert.Equal(DependencyProperty.UnsetValue, Converter.Convert(null!, typeof(string), null!, ""));
    }

    [Theory]
    [InlineData("en-US", "1,234.50")]
    [InlineData("de-DE", "1.234,50")]
    public void Parses_Back_In_The_Same_Language(string language, string text)
    {
        Assert.Equal(1234.5d, Converter.ConvertBack(text, typeof(double), null!, language));
    }

    [Fact]
    public void Text_That_Does_Not_Parse_Is_Left_Unset()
    {
        Assert.Equal(DependencyProperty.UnsetValue, Converter.ConvertBack("twelve", typeof(double), null!, "en-US"));
    }

    [Fact]
    public void Round_Trips_Through_Both_Directions()
    {
        var text = Converter.Convert(98.76d, typeof(string), null!, "de-DE");

        Assert.Equal(98.76d, Converter.ConvertBack(text, typeof(double), null!, "de-DE"));
    }
}
