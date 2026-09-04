using System.Globalization;
using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex024_NumericInputParsingTests : BunitContext
{
    [Fact]
    public void Changing_The_Quantity_Updates_The_Total()
    {
        var cut = Render<Ex024_NumericInputParsing>(p => p.Add(c => c.UnitPrice, 2.5m));

        cut.Find("#qty").Change("3");

        cut.WaitForAssertion(() => Assert.Equal("7.5", cut.Find("#total").TextContent));
    }

    [Fact]
    public void With_No_Interaction_The_Total_Is_Zero()
    {
        var cut = Render<Ex024_NumericInputParsing>(p => p.Add(c => c.UnitPrice, 2.5m));

        Assert.Equal("0", cut.Find("#total").TextContent);
    }

    [Fact]
    public void Unparsable_Input_Leaves_The_Quantity_And_Total_Unchanged()
    {
        var cut = Render<Ex024_NumericInputParsing>(p => p.Add(c => c.UnitPrice, 2.5m));

        cut.Find("#qty").Change("3");
        cut.WaitForAssertion(() => Assert.Equal("7.5", cut.Find("#total").TextContent));

        cut.Find("#qty").Change("abc");

        cut.WaitForAssertion(() => Assert.Equal("7.5", cut.Find("#total").TextContent));
    }

    [Fact]
    public void Under_A_Decimal_Comma_Culture_The_Total_Still_Uses_A_Decimal_Point()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var cut = Render<Ex024_NumericInputParsing>(p => p.Add(c => c.UnitPrice, 2.5m));
            cut.Find("#qty").Change("3");

            // A total of "7,5" (decimal comma) is invalid for this span's
            // contract - this is what a missing CultureInfo.InvariantCulture
            // would silently produce.
            cut.WaitForAssertion(() => Assert.Equal("7.5", cut.Find("#total").TextContent));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
