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
    public void Formatting_And_Parsing_Are_Invariant_Not_Current_Culture()
    {
        // th-TH formats with a Buddhist-calendar year (2569, not 2026) and misparses
        // the same text under that era, unlike de-DE - which only reorders/repunctuates
        // and so cannot catch a missing InvariantCulture argument on either side. This
        // exercises both InvariantCulture arguments, not just the formatting one.
        // Caveat (already true of the de-DE version this replaces): if
        // InvariantGlobalization is ever turned on for this project,
        // CultureInfo.GetCultureInfo("th-TH") degrades to the invariant culture and
        // this fact goes quiet.
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("th-TH");

            var current = new DateOnly(2026, 9, 4);
            var bound = Render<Ex009_BindFormat>(p => p.Bind(c => c.Value, current, v => current = v));

            Assert.Equal("2026-09-04", bound.Find("#due").GetAttribute("value")); // 2569-09-04 without InvariantCulture

            bound.Find("#due").Change("2026-12-24");
            Assert.Equal(new DateOnly(2026, 12, 24), current); // 1483-12-24 without InvariantCulture
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
