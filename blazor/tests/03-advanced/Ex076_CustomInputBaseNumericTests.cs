using System.Globalization;
using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

using RateModel = Ex076_CustomInputBaseNumeric_Form.RateModel;

public class Ex076_CustomInputBaseNumericTests : BunitContext
{
    private readonly RateModel _model = new() { Rate = 0.15m };

    private IRenderedComponent<Ex076_CustomInputBaseNumeric_Form> RenderForm()
        => Render<Ex076_CustomInputBaseNumeric_Form>(p => p.Add(c => c.Model, _model));

    [Fact]
    public void Shows_The_Stored_Fraction_As_A_Percentage()
    {
        var cut = RenderForm();

        Assert.Equal("15", cut.Find("#rate").GetAttribute("value"));
    }

    [Fact]
    public void Typing_A_Percentage_Stores_The_Fraction()
    {
        var cut = RenderForm();

        cut.Find("#rate").Change("20");

        cut.WaitForAssertion(() => Assert.Equal(0.20m, _model.Rate));
        Assert.Empty(cut.FindAll(".validation-message"));
    }

    // The value-type difference from ex075, and the whole reason this row exists
    // separately: a string[] has an empty value, a decimal does not, so clearing the
    // field is an error rather than a way of saying "nothing".
    [Fact]
    public void Clearing_The_Input_Is_An_Error_Not_An_Empty_Value()
    {
        var cut = RenderForm();

        cut.Find("#rate").Change("");

        cut.WaitForAssertion(() => Assert.Equal(
            Ex076_CustomInputBaseNumeric.ParseError,
            cut.Find(".validation-message").TextContent));
        Assert.Equal(0.15m, _model.Rate);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("-1")]
    [InlineData("101")]
    public void Junk_And_Out_Of_Range_Values_Are_Reported_And_Do_Not_Land(string typed)
    {
        var cut = RenderForm();

        cut.Find("#rate").Change(typed);

        cut.WaitForAssertion(() => Assert.Equal(
            Ex076_CustomInputBaseNumeric.ParseError,
            cut.Find(".validation-message").TextContent));
        Assert.Equal(0.15m, _model.Rate);
    }

    [Fact]
    public void The_Bounds_Themselves_Are_Accepted()
    {
        var cut = RenderForm();

        cut.Find("#rate").Change("100");
        cut.WaitForAssertion(() => Assert.Equal(1m, _model.Rate));

        cut.Find("#rate").Change("0");
        cut.WaitForAssertion(() => Assert.Equal(0m, _model.Rate));
    }

    // Both directions are pinned to InvariantCulture, so a de-DE machine still reads
    // "12.5" as twelve and a half rather than as a hundred and twenty-five, and still
    // writes a dot rather than a comma. Same lesson as ex009, on the InputBase seam.
    [Fact]
    public void Formatting_And_Parsing_Ignore_The_Ambient_Culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var cut = RenderForm();
            cut.Find("#rate").Change("12.5");

            cut.WaitForAssertion(() => Assert.Equal(0.125m, _model.Rate)); // 1.25 under de-DE
            Assert.Equal("12.5", cut.Find("#rate").GetAttribute("value")); // "12,5" under de-DE
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
