using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex094_CustomComponentBaseRenderHookTests : BunitContext
{
    [Fact]
    public void The_First_Push_Renders_And_Has_No_Previous_Value()
    {
        var cut = Render<Ex094_CustomComponentBaseRenderHook>(p => p.Add(c => c.Text, "one"));

        Assert.Equal("one", cut.Find("#text").TextContent);
        Assert.Equal(1, cut.Instance.ParameterSets);
        Assert.Null(cut.Instance.PreviousText);
    }

    // Ruling: the ordering. PreviousText is only meaningful if it is captured before
    // SetParameterProperties overwrites Text - afterwards both sides are "two" and
    // the hook can never see a change.
    [Fact]
    public void A_Later_Push_Sees_What_The_Previous_One_Carried()
    {
        var cut = Render<Ex094_CustomComponentBaseRenderHook>(p => p.Add(c => c.Text, "one"));

        cut.Render(p => p.Add(c => c.Text, "two"));

        Assert.Equal("two", cut.Instance.Text);
        Assert.Equal("one", cut.Instance.PreviousText);
        Assert.Equal(2, cut.Instance.ParameterSets);
    }

    [Fact]
    public void The_After_Render_Hook_Runs_For_Every_Render()
    {
        var cut = Render<Ex094_CustomComponentBaseRenderHook>(p => p.Add(c => c.Text, "one"));
        Assert.Equal(1, cut.Instance.AfterRenders);

        cut.Render(p => p.Add(c => c.Text, "two"));

        Assert.Equal(2, cut.Instance.AfterRenders);
    }

    [Fact]
    public void The_Once_Only_Setup_Happens_On_The_First_Pass()
    {
        var cut = Render<Ex094_CustomComponentBaseRenderHook>(p => p.Add(c => c.Text, "one"));

        Assert.True(cut.Instance.SetupDone);
        Assert.Equal(1, cut.Instance.AfterRenders);
    }
}
