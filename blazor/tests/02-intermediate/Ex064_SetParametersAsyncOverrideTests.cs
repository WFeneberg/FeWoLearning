using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex064_SetParametersAsyncOverrideTests : BunitContext
{
    // Every assertion here follows a synchronous parameter push, which completes
    // before Render returns - no WaitForAssertion needed (README §11).
    [Fact]
    public void The_First_Push_Is_One_Set_And_One_Change()
    {
        var cut = Render<Ex064_SetParametersAsyncOverride>(p => p.Add(c => c.Value, "a"));

        Assert.Equal("a", cut.Find("#value").TextContent);
        Assert.Equal("1", cut.Find("#sets").TextContent);
        Assert.Equal("1", cut.Find("#changes").TextContent);
    }

    // The distinction the override exists to make: a push that carries the same value
    // is still a set, but not a change.
    [Fact]
    public void Re_Pushing_The_Same_Value_Counts_As_A_Set_But_Not_A_Change()
    {
        var cut = Render<Ex064_SetParametersAsyncOverride>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "a"));

        Assert.Equal("2", cut.Find("#sets").TextContent);
        Assert.Equal("1", cut.Find("#changes").TextContent);
    }

    // Non-vacuity for the ordering: comparing after base.SetParametersAsync has
    // already assigned Value makes incoming and current equal every time, so
    // ChangeCount never leaves 0 - measured, and it takes all four facts in this
    // class red at once. Calling base at all is what assigns Value, so a missing
    // base call shows up in #value here.
    [Fact]
    public void A_Different_Value_Counts_As_A_Change_And_Still_Lands()
    {
        var cut = Render<Ex064_SetParametersAsyncOverride>(p => p.Add(c => c.Value, "a"));

        cut.Render(p => p.Add(c => c.Value, "b"));

        Assert.Equal("b", cut.Find("#value").TextContent);
        Assert.Equal("2", cut.Find("#sets").TextContent);
        Assert.Equal("2", cut.Find("#changes").TextContent);
    }

    [Fact]
    public void A_Push_That_Omits_The_Parameter_Is_A_Set_Without_A_Change()
    {
        var cut = Render<Ex064_SetParametersAsyncOverride>(p => p.Add(c => c.Value, "a"));

        cut.Render(_ => { });

        Assert.Equal("2", cut.Find("#sets").TextContent);
        Assert.Equal("1", cut.Find("#changes").TextContent);
    }
}
