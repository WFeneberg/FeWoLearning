using System.Globalization;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex095_WpfValueConverterTests
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    [Fact]
    public void ConvertsTrueToVisible()
    {
        var converter = new WpfValueConverter();
        var result = converter.Convert(true, typeof(Visibility), null, Culture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void ConvertsFalseToCollapsed()
    {
        var converter = new WpfValueConverter();
        var result = converter.Convert(false, typeof(Visibility), null, Culture);
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void InvertParameterFlipsConvertMapping()
    {
        var converter = new WpfValueConverter();
        Assert.Equal(Visibility.Collapsed, converter.Convert(true, typeof(Visibility), "Invert", Culture));
        Assert.Equal(Visibility.Visible, converter.Convert(false, typeof(Visibility), "Invert", Culture));
    }

    [Fact]
    public void ConvertBackVisibleToTrue()
    {
        var converter = new WpfValueConverter();
        var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, Culture);
        Assert.Equal(true, result);
    }

    [Theory]
    [InlineData(Visibility.Hidden)]
    [InlineData(Visibility.Collapsed)]
    public void ConvertBackNonVisibleToFalse(Visibility visibility)
    {
        var converter = new WpfValueConverter();
        var result = converter.ConvertBack(visibility, typeof(bool), null, Culture);
        Assert.Equal(false, result);
    }

    [Fact]
    public void InvertParameterFlipsConvertBackMapping()
    {
        var converter = new WpfValueConverter();
        Assert.Equal(false, converter.ConvertBack(Visibility.Visible, typeof(bool), "Invert", Culture));
        Assert.Equal(true, converter.ConvertBack(Visibility.Collapsed, typeof(bool), "Invert", Culture));
    }

    [Fact]
    public void RoundTripsThroughConvertAndConvertBack()
    {
        var converter = new WpfValueConverter();
        var visible = converter.Convert(true, typeof(Visibility), null, Culture);
        var back = converter.ConvertBack(visible, typeof(bool), null, Culture);
        Assert.Equal(true, back);
    }

    [Fact]
    public void ConvertReturnsDoNothingForNonBoolInput()
    {
        var converter = new WpfValueConverter();
        var result = converter.Convert("not a bool", typeof(Visibility), null, Culture);
        Assert.Same(WpfValueConverter.DoNothing, result);
    }

    [Fact]
    public void ConvertReturnsDoNothingForNullInput()
    {
        var converter = new WpfValueConverter();
        var result = converter.Convert(null, typeof(Visibility), null, Culture);
        Assert.Same(WpfValueConverter.DoNothing, result);
    }

    [Fact]
    public void ConvertBackReturnsDoNothingForNonVisibilityInput()
    {
        var converter = new WpfValueConverter();
        var result = converter.ConvertBack(42, typeof(bool), null, Culture);
        Assert.Same(WpfValueConverter.DoNothing, result);
    }
}
