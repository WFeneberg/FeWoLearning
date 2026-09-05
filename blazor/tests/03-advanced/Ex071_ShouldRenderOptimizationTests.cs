using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex071_ShouldRenderOptimizationTests : BunitContext
{
    // The point of the exercise: a push carrying nothing new costs nothing. The
    // first-render assertions ride along here rather than forming a fact of their
    // own - ComponentBase does not consult ShouldRender for the first render, so a
    // fact that only checked it would pass against the untouched stub.
    // Negative assertion, so it stays bare (README §11).
    [Fact]
    public void Re_Pushing_The_Same_Value_Does_Not_Render_Again()
    {
        var cut = Render<Ex071_ShouldRenderOptimization>(p => p.Add(c => c.Value, "a"));
        Assert.Equal("a", cut.Find("#value").TextContent);
        Assert.Equal(1, cut.Instance.RenderCount);

        cut.Render(p => p.Add(c => c.Value, "a"));
        cut.Render(p => p.Add(c => c.Value, "a"));

        Assert.Equal(1, cut.Instance.RenderCount);
    }

    [Fact]
    public void A_Changed_Value_Renders_And_Shows_Up()
    {
        var cut = Render<Ex071_ShouldRenderOptimization>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "b"));

        Assert.Equal("b", cut.Find("#value").TextContent);
        Assert.Equal(2, cut.Instance.RenderCount);
    }

    // The trap: ShouldRender gates every render, not only parameter-driven ones. A
    // gate that only asks "did the parameter change?" swallows this click - the
    // handler runs, _clicks goes up, and the screen never says so.
    [Fact]
    public void A_Click_Still_Renders_Even_Though_The_Parameter_Did_Not_Change()
    {
        var cut = Render<Ex071_ShouldRenderOptimization>(p => p.Add(c => c.Value, "a"));

        cut.Find("#bump").Click();

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#clicks").TextContent));
        Assert.Equal(2, cut.Instance.RenderCount);
    }

    // And the gate closes again afterwards: the click's render must not leave the
    // component permanently dirty.
    [Fact]
    public void The_Gate_Closes_Again_After_A_Click()
    {
        var cut = Render<Ex071_ShouldRenderOptimization>(p => p.Add(c => c.Value, "a"));
        cut.Find("#bump").Click();
        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#clicks").TextContent));

        cut.Render(p => p.Add(c => c.Value, "a"));

        Assert.Equal(2, cut.Instance.RenderCount);
    }
}
