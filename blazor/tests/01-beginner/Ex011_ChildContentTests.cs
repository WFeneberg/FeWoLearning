using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex011_ChildContentTests : BunitContext
{
    [Fact]
    public void Title_And_Child_Content_Both_Render_When_Child_Content_Is_Supplied()
    {
        var cut = Render<Ex011_ChildContent>(p => p
            .Add(c => c.Title, "Card")
            .AddChildContent("<p>inner</p>"));

        Assert.Equal("Card", cut.Find(".card-title").TextContent);
        Assert.Equal("inner", cut.Find(".card-body p").TextContent);
    }

    [Fact]
    public void Card_Body_Is_Omitted_When_Child_Content_Is_Absent()
    {
        var cut = Render<Ex011_ChildContent>(p => p.Add(c => c.Title, "Card"));

        Assert.Empty(cut.FindAll(".card-body"));

        // Pre-state sanity, folded in here rather than as a standalone fact: the
        // title still renders with no child content supplied. On its own this
        // would pass the moment any card markup exists at all, whether or not the
        // body is correctly guarded - only the assertion above (that the body div
        // itself is gone) proves the guard actually runs.
        Assert.Equal("Card", cut.Find(".card-title").TextContent);
    }
}
