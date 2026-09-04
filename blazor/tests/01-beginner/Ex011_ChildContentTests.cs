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

        // Folded in here rather than as a standalone fact: its premise (Title set,
        // no ChildContent supplied) is identical to the setup above, so the
        // assertion is merged into this test instead of duplicating that setup.
        Assert.Equal("Card", cut.Find(".card-title").TextContent);
    }
}
