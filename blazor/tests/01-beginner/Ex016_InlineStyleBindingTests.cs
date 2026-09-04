using System.Globalization;
using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex016_InlineStyleBindingTests : BunitContext
{
    [Fact]
    public void Percent_42_Produces_A_42_Percent_Width()
    {
        var cut = Render<Ex016_InlineStyleBinding>(p => p.Add(c => c.Percent, 42.0));

        Assert.Equal("width: 42%", cut.Find("#track #bar").GetAttribute("style"));
    }

    [Fact]
    public void Negative_Percent_Clamps_To_Zero()
    {
        var cut = Render<Ex016_InlineStyleBinding>(p => p.Add(c => c.Percent, -5.0));

        Assert.Equal("width: 0%", cut.Find("#track #bar").GetAttribute("style"));
    }

    [Fact]
    public void Percent_Over_100_Clamps_To_100()
    {
        var cut = Render<Ex016_InlineStyleBinding>(p => p.Add(c => c.Percent, 150.0));

        Assert.Equal("width: 100%", cut.Find("#track #bar").GetAttribute("style"));
    }

    [Fact]
    public void Fractional_Percent_Formats_With_A_Decimal_Point()
    {
        var cut = Render<Ex016_InlineStyleBinding>(p => p.Add(c => c.Percent, 42.5));

        Assert.Equal("width: 42.5%", cut.Find("#track #bar").GetAttribute("style"));
    }

    [Fact]
    public void Under_A_Decimal_Comma_Culture_The_Style_Still_Uses_A_Decimal_Point()
    {
        var original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        try
        {
            var cut = Render<Ex016_InlineStyleBinding>(p => p.Add(c => c.Percent, 42.5));

            // A style="width: 42,5%" (decimal comma) is invalid CSS - this is
            // what a missing CultureInfo.InvariantCulture would silently produce.
            Assert.Equal("width: 42.5%", cut.Find("#track #bar").GetAttribute("style"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
