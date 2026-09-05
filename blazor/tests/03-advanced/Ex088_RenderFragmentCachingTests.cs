using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex088_RenderFragmentCachingTests : BunitContext
{
    private static int PanelRenders(IRenderedComponent<Ex088_RenderFragmentCaching> cut)
        => cut.FindComponent<Ex088_RenderFragmentCaching_Panel>().Instance.RenderCount;

    [Fact]
    public void The_Panel_Renders_The_Fragments_Content()
    {
        var cut = Render<Ex088_RenderFragmentCaching>(p => p.Add(c => c.Label, "a"));

        Assert.Equal("a", cut.Find(".label").TextContent);
        Assert.Equal("panel body", cut.Find(".body").TextContent);
    }

    // The mechanism itself, said directly: same instance, render after render.
    [Fact]
    public void The_Same_Fragment_Instance_Comes_Back_Every_Time()
    {
        var cut = Render<Ex088_RenderFragmentCaching>(p => p.Add(c => c.Label, "a"));
        var first = cut.Instance.Content;

        cut.Render(p => p.Add(c => c.Label, "b"));

        Assert.Same(first, cut.Instance.Content);
    }

    // Ruling: what the caching buys. The panel gates on its parameters, so an
    // unchanged push costs nothing - but a fragment rebuilt inline would be a new
    // delegate each time and the gate could never close. Negative assertion, bare.
    [Fact]
    public void An_Unchanged_Push_Does_Not_Re_Render_The_Panel()
    {
        var cut = Render<Ex088_RenderFragmentCaching>(p => p.Add(c => c.Label, "a"));
        Assert.Equal(1, PanelRenders(cut));

        cut.Render(p => p.Add(c => c.Label, "a"));
        cut.Render(p => p.Add(c => c.Label, "a"));

        Assert.Equal(1, PanelRenders(cut));
    }

    // Non-vacuity: the gate must still open for a real change, or the fact above
    // would be satisfied by a panel that simply never renders again.
    [Fact]
    public void A_Changed_Label_Still_Re_Renders_The_Panel()
    {
        var cut = Render<Ex088_RenderFragmentCaching>(p => p.Add(c => c.Label, "a"));

        cut.Render(p => p.Add(c => c.Label, "b"));

        Assert.Equal("b", cut.Find(".label").TextContent);
        Assert.Equal(2, PanelRenders(cut));
    }
}
