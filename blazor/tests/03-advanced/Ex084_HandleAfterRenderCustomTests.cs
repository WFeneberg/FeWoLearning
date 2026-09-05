using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex084_HandleAfterRenderCustomTests : BunitContext
{
    [Fact]
    public void The_Hook_Runs_After_The_First_Render()
    {
        var cut = Render<Ex084_HandleAfterRenderCustom>(p => p.Add(c => c.Value, "a"));

        Assert.Equal(1, cut.Instance.AfterRenders);
        Assert.Equal(1, cut.Instance.SetupRuns);
    }

    [Fact]
    public void It_Runs_Again_After_Every_Later_Render()
    {
        var cut = Render<Ex084_HandleAfterRenderCustom>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "b"));
        cut.Render(p => p.Add(c => c.Value, "c"));

        Assert.Equal("c", cut.Find("#value").TextContent);
        Assert.Equal(3, cut.Instance.AfterRenders);
    }

    // Owning the dispatch means owning the "first time only" decision too: there is
    // no firstRender flag to lean on any more.
    [Fact]
    public void The_Once_Only_Work_Stays_Once_Only()
    {
        var cut = Render<Ex084_HandleAfterRenderCustom>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "b"));
        cut.Render(p => p.Add(c => c.Value, "c"));

        Assert.Equal(1, cut.Instance.SetupRuns);
    }

    // Ruling: the sharp edge of this row. IHandleAfterRender.OnAfterRenderAsync IS
    // the dispatch - ComponentBase implements it and calls OnAfterRender from there.
    // Re-implementing the interface replaces that implementation wholesale, so
    // OnAfterRender is never reached, however ordinary the override looks. Measured
    // directly. Negative assertion, so it stays bare.
    [Fact]
    public void Taking_Over_The_Interface_Silences_ComponentBases_Own_Hook()
    {
        var cut = Render<Ex084_HandleAfterRenderCustom>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "b"));

        Assert.Equal(0, cut.Instance.BaseHookRuns);
        Assert.Equal(2, cut.Instance.AfterRenders);
    }
}
