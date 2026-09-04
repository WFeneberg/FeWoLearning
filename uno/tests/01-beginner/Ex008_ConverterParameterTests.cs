using System.Globalization;
using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex008_ConverterParameterTests : UnoTestContext
{
    private static readonly IValueConverter Converter = new Ex008_ConverterParameter();

    private static object Convert(object value, object parameter) =>
        Converter.Convert(value, typeof(Visibility), parameter, "");

    [Fact]
    public void At_Or_Above_The_Threshold_Is_Visible()
    {
        Assert.Equal(Visibility.Visible, Convert(3d, "2.5"));
        Assert.Equal(Visibility.Visible, Convert(2.5d, "2.5"));
    }

    [Fact]
    public void Below_The_Threshold_Is_Collapsed()
    {
        Assert.Equal(Visibility.Collapsed, Convert(1d, "2.5"));
    }

    [Fact]
    public void The_Threshold_May_Arrive_Already_Typed()
    {
        // From markup it is a string; from a code-behind binding it can be a double.
        Assert.Equal(Visibility.Visible, Convert(3d, 2.5d));
        Assert.Equal(Visibility.Collapsed, Convert(1d, 2.5d));
    }

    [Fact]
    public void No_Threshold_Shows_Everything()
    {
        Assert.Equal(Visibility.Visible, Convert(0d, null!));
        Assert.Equal(Visibility.Visible, Convert(0d, "not a number"));
    }

    [Fact]
    public void The_Threshold_Is_Read_The_Same_Way_On_Every_Machine()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            // On a German machine "2.5" parses as 25 under CurrentCulture, and everything
            // below 25 silently disappears. The markup literal is invariant, always.
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            Assert.Equal(Visibility.Visible, Convert(3d, "2.5"));
            Assert.Equal(Visibility.Collapsed, Convert(2d, "2.5"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Converting_Back_Is_Refused_Rather_Than_Guessed()
    {
        Assert.Throws<NotSupportedException>(
            () => Converter.ConvertBack(Visibility.Visible, typeof(double), "2.5", ""));
    }
}
