using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex092_RenderTreeBuilderConditionalTests : BunitContext
{
    private IRenderedComponent<Ex092_RenderTreeBuilderConditional> RenderAt(bool editing)
        => Render<Ex092_RenderTreeBuilderConditional>(p => p
            .Add(c => c.Editing, editing)
            .Add(c => c.Text, "hello"));

    private static Ex092_RenderTreeBuilderConditional_Badge Badge(
        IRenderedComponent<Ex092_RenderTreeBuilderConditional> cut)
        => cut.FindComponent<Ex092_RenderTreeBuilderConditional_Badge>().Instance;

    [Fact]
    public void The_Viewer_Branch_Renders_A_Span()
    {
        var cut = RenderAt(editing: false);

        Assert.Equal("hello", cut.Find(".viewer").TextContent);
        Assert.Empty(cut.FindAll(".editor"));
    }

    [Fact]
    public void The_Editor_Branch_Renders_An_Input()
    {
        var cut = RenderAt(editing: true);

        Assert.Equal("hello", cut.Find(".editor").GetAttribute("value"));
        Assert.Empty(cut.FindAll(".viewer"));
    }

    [Fact]
    public void Toggling_Swaps_The_Branch()
    {
        var cut = RenderAt(editing: false);

        cut.Render(p => p.Add(c => c.Editing, true).Add(c => c.Text, "hello"));

        Assert.NotEmpty(cut.FindAll(".editor"));
        Assert.Empty(cut.FindAll(".viewer"));
    }

    // Ruling: the row itself. Both branches open the badge at the same sequence
    // number, so the diff treats it as one component that stayed put while its
    // neighbour changed - the instance, and the state only it holds, survive the
    // toggle. Give the two branches different numbers and the diff sees an old
    // component going away and a new one arriving; measured directly by mutation.
    [Fact]
    public void The_Badge_Survives_The_Branch_Toggle_With_Its_State()
    {
        var cut = RenderAt(editing: false);
        var before = Badge(cut);
        cut.InvokeAsync(() => before.Tick());
        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find(".badge").GetAttribute("data-ticks")));

        cut.Render(p => p.Add(c => c.Editing, true).Add(c => c.Text, "hello"));

        Assert.Same(before, Badge(cut));
        Assert.Equal(1, Badge(cut).Ticks);
        Assert.Equal("1", cut.Find(".badge").GetAttribute("data-ticks"));
    }

    [Fact]
    public void The_Badge_Follows_The_Text_Across_The_Toggle()
    {
        var cut = RenderAt(editing: false);

        cut.Render(p => p.Add(c => c.Editing, true).Add(c => c.Text, "changed"));

        Assert.Equal("changed", cut.Find(".badge").TextContent);
    }
}
