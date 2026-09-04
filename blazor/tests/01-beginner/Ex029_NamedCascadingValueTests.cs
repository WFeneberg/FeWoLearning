using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex029_NamedCascadingValueTests : BunitContext
{
    [Fact]
    public void The_Label_Consumes_Both_Named_Values_By_Their_Own_Names()
    {
        var cut = Render<Ex029_NamedCascadingValue>(p => p
            .Add(c => c.Locale, "de-DE")
            .Add(c => c.Currency, "EUR")
            .AddChildContent<Ex029_NamedCascadingValue_Label>());

        Assert.Equal("de-DE", cut.Find("#locale").TextContent);
        Assert.Equal("EUR", cut.Find("#currency").TextContent);
    }

    [Fact]
    public void Swapping_The_Provider_Values_Swaps_Which_Name_Sees_Which_Value()
    {
        var cut = Render<Ex029_NamedCascadingValue>(p => p
            .Add(c => c.Locale, "EUR")
            .Add(c => c.Currency, "de-DE")
            .AddChildContent<Ex029_NamedCascadingValue_Label>());

        Assert.Equal("EUR", cut.Find("#locale").TextContent);
    }
}
