using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex004_ListRenderingTests : BunitContext
{
    [Fact]
    public void Renders_One_Li_Per_Tag_In_Order()
    {
        var cut = Render<Ex004_ListRendering>(p => p.Add(c => c.Tags, new[] { "a", "b", "c" }));

        var texts = cut.FindAll("#tags li.tag").Select(e => e.TextContent).ToArray();
        Assert.Equal(new[] { "a", "b", "c" }, texts);
    }

    [Fact]
    public void Renders_The_Ul_With_No_Items_When_The_List_Is_Empty()
    {
        var cut = Render<Ex004_ListRendering>(p => p.Add(c => c.Tags, Array.Empty<string>()));

        Assert.Equal("UL", cut.Find("#tags").TagName);
        Assert.Empty(cut.FindAll("#tags li.tag"));
    }

    [Fact]
    public void Does_Not_Collapse_Duplicate_Tags()
    {
        var cut = Render<Ex004_ListRendering>(p => p.Add(c => c.Tags, new[] { "x", "x" }));

        Assert.Equal(2, cut.FindAll("#tags li.tag").Count);
    }
}
