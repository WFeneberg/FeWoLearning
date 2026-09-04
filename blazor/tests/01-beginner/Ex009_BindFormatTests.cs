using System.Globalization;
using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex009_BindFormatTests : BunitContext
{
    [Fact]
    public void Renders_The_Date_As_Yyyy_Mm_Dd()
    {
        var cut = Render<Ex009_BindFormat>(p => p.Add(c => c.Value, new DateOnly(2026, 9, 4)));

        Assert.Equal("2026-09-04", cut.Find("#due").GetAttribute("value"));
    }

    [Fact]
    public void Bound_Change_With_A_Valid_Date_Parses_And_Flows_Back()
    {
        var current = new DateOnly(2026, 9, 4);
        var cut = Render<Ex009_BindFormat>(p => p.Bind(c => c.Value, current, v => current = v));

        cut.Find("#due").Change("2026-12-24");

        Assert.Equal(new DateOnly(2026, 12, 24), current);
    }

    [Fact]
    public void Bound_Change_With_Unparsable_Text_Leaves_The_Value_Unchanged_And_Does_Not_Throw()
    {
        var current = new DateOnly(2026, 9, 4);
        var cut = Render<Ex009_BindFormat>(p => p.Bind(c => c.Value, current, v => current = v));

        cut.Find("#due").Change("not-a-date");

        Assert.Equal(new DateOnly(2026, 9, 4), current);
    }

    [Fact]
    public void Formatting_Is_Invariant_Not_Current_Culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var cut = Render<Ex009_BindFormat>(p => p.Add(c => c.Value, new DateOnly(2026, 9, 4)));

            Assert.Equal("2026-09-04", cut.Find("#due").GetAttribute("value"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
