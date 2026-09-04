using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex033_EmptyStateFallbackTests : BunitContext
{
    [Fact]
    public void Blank_Query_Shows_The_Prompt()
    {
        var cut = Render<Ex033_EmptyStateFallback>(p => p.Add(c => c.Query, ""));

        Assert.Equal("Type to search", cut.Find("#prompt").TextContent);
        Assert.Empty(cut.FindAll("#results"));
        Assert.Empty(cut.FindAll("#no-results"));
    }

    [Fact]
    public void Non_Blank_Query_With_Results_Renders_The_Hit_List()
    {
        var cut = Render<Ex033_EmptyStateFallback>(p => p
            .Add(c => c.Query, "bl")
            .Add(c => c.Results, new[] { "blazor", "blue" }));

        var hits = cut.FindAll("#results li.hit");
        Assert.Equal(new[] { "blazor", "blue" }, hits.Select(h => h.TextContent).ToArray());
        Assert.Empty(cut.FindAll("#prompt"));
        Assert.Empty(cut.FindAll("#no-results"));
    }

    [Fact]
    public void Non_Blank_Query_With_No_Results_Shows_The_Exact_No_Results_Copy()
    {
        var cut = Render<Ex033_EmptyStateFallback>(p => p
            .Add(c => c.Query, "zzz")
            .Add(c => c.Results, Array.Empty<string>()));

        Assert.Equal("No results for \"zzz\"", cut.Find("#no-results").TextContent);
    }

    [Fact]
    public void A_Whitespace_Only_Query_Counts_As_No_Query()
    {
        // Same premise as fact 1 (blank query -> prompt state): "   " must be
        // treated as blank, not as a query with zero results.
        var cut = Render<Ex033_EmptyStateFallback>(p => p
            .Add(c => c.Query, "   ")
            .Add(c => c.Results, Array.Empty<string>()));

        Assert.Equal("P", cut.Find("#prompt").TagName);
        Assert.Empty(cut.FindAll("#no-results"));
    }
}
