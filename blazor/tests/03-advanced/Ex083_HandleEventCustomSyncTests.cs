using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex083_HandleEventCustomSyncTests : BunitContext
{
    // The whole point: the handler ran, the state moved, and the screen did not.
    // Negative assertion on markup, so it stays bare (README §11).
    [Fact]
    public void A_Click_Runs_The_Handler_Without_Rendering()
    {
        var cut = Render<Ex083_HandleEventCustomSync>();

        cut.Find("#bump").Click();

        Assert.Equal(1, cut.Instance.Count);
        Assert.Equal("0", cut.Find("#count").TextContent);
        Assert.Equal(1, cut.Instance.RenderCount);
    }

    [Fact]
    public void Several_Clicks_Accumulate_Behind_The_Stale_Markup()
    {
        var cut = Render<Ex083_HandleEventCustomSync>();

        cut.Find("#bump").Click();
        cut.Find("#bump").Click();
        cut.Find("#bump").Click();

        Assert.Equal(3, cut.Instance.Count);
        Assert.Equal("0", cut.Find("#count").TextContent);
        Assert.Equal(1, cut.Instance.RenderCount);
    }

    [Fact]
    public void The_Explicit_Render_Catches_The_Markup_Up()
    {
        var cut = Render<Ex083_HandleEventCustomSync>();
        cut.Find("#bump").Click();
        cut.Find("#bump").Click();

        cut.Find("#show").Click();

        cut.WaitForAssertion(() => Assert.Equal("2", cut.Find("#count").TextContent));
        Assert.Equal(2, cut.Instance.RenderCount);
    }

    // Non-vacuity for "the pipeline is the component's, not one handler's": Show()
    // goes through the same IHandleEvent, so it only renders because it asks to. A
    // Show() that forgot StateHasChanged would leave the markup stale here, and an
    // IHandleEvent that still rendered would make the first fact above fail instead.
    [Fact]
    public void Rendering_Happens_Only_When_Asked_For()
    {
        var cut = Render<Ex083_HandleEventCustomSync>();

        cut.Find("#show").Click();
        cut.WaitForAssertion(() => Assert.Equal(2, cut.Instance.RenderCount));

        cut.Find("#bump").Click();

        Assert.Equal(2, cut.Instance.RenderCount);
    }
}
