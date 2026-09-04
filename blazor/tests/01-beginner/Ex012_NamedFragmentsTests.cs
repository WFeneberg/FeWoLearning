using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex012_NamedFragmentsTests : BunitContext
{
    [Fact]
    public void All_Three_Regions_Render_Their_Own_Content_When_All_Fragments_Are_Supplied()
    {
        var cut = Render<Ex012_NamedFragments>(p => p
            .Add(c => c.Header, "<p>head</p>")
            .Add(c => c.Body, "<p>body</p>")
            .Add(c => c.Footer, "<p>foot</p>"));

        Assert.Equal("head", cut.Find("#dialog-header").TextContent);
        Assert.Equal("body", cut.Find("#dialog-body").TextContent);
        Assert.Equal("foot", cut.Find("#dialog-footer").TextContent);
    }

    [Fact]
    public void Only_The_Supplied_Region_Renders_When_Just_Body_Is_Given()
    {
        var cut = Render<Ex012_NamedFragments>(p => p.Add(c => c.Body, "<p>body</p>"));

        Assert.Equal("body", cut.Find("#dialog-body").TextContent);
        Assert.Empty(cut.FindAll("#dialog-header"));
        Assert.Empty(cut.FindAll("#dialog-footer"));
    }

    [Fact]
    public void The_Dialog_Renders_With_No_Regions_When_No_Fragments_Are_Supplied()
    {
        var cut = Render<Ex012_NamedFragments>();

        Assert.Equal("DIV", cut.Find(".dialog").TagName);
        Assert.Empty(cut.FindAll("#dialog-header"));
        Assert.Empty(cut.FindAll("#dialog-body"));
        Assert.Empty(cut.FindAll("#dialog-footer"));
    }
}
