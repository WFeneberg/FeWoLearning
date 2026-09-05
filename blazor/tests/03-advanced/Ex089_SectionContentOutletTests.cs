using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex089_SectionContentOutletTests : BunitContext
{
    private IRenderedComponent<Ex089_SectionContentOutlet> RenderPage(
        string tools = "cut copy paste", string status = "ready", bool showTools = true)
        => Render<Ex089_SectionContentOutlet>(p => p
            .Add(c => c.Tools, tools)
            .Add(c => c.StatusText, status)
            .Add(c => c.ShowTools, showTools));

    // The whole point: declared inside #page, rendered inside #toolbar - and #page
    // itself stays empty, which is what projection means here.
    [Fact]
    public void Content_Renders_At_Its_Outlet_Not_Where_It_Is_Declared()
    {
        var cut = RenderPage();

        Assert.Equal("cut copy paste", cut.Find("#toolbar #tools").TextContent);
        Assert.Empty(cut.Find("#page").Children);
    }

    // Two outlets, two names: the pairing is by SectionName, not by order or nesting.
    [Fact]
    public void Each_Section_Name_Reaches_Its_Own_Outlet()
    {
        var cut = RenderPage(status: "saving");

        Assert.Equal("saving", cut.Find("#status #state").TextContent);
        Assert.Empty(cut.FindAll("#toolbar #state"));
        Assert.Empty(cut.FindAll("#status #tools"));
    }

    [Fact]
    public void Updating_The_Content_Updates_The_Outlet()
    {
        var cut = RenderPage(tools: "cut copy paste");

        cut.Render(p => p
            .Add(c => c.Tools, "undo redo")
            .Add(c => c.StatusText, "ready")
            .Add(c => c.ShowTools, true));

        Assert.Equal("undo redo", cut.Find("#toolbar #tools").TextContent);
    }

    // An outlet with nothing supplying it renders nothing - it is a hole, not a
    // placeholder. Negative assertion, so it stays bare.
    [Fact]
    public void Removing_The_Content_Empties_The_Outlet()
    {
        var cut = RenderPage();
        Assert.NotEmpty(cut.FindAll("#toolbar #tools"));

        cut.Render(p => p
            .Add(c => c.Tools, "cut copy paste")
            .Add(c => c.StatusText, "ready")
            .Add(c => c.ShowTools, false));

        Assert.Empty(cut.Find("#toolbar").Children);
        Assert.Equal("ready", cut.Find("#status #state").TextContent);
    }
}
