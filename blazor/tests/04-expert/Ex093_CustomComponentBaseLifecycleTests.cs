using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex093_CustomComponentBaseLifecycleTests : BunitContext
{
    [Fact]
    public void A_Parameter_Push_Applies_The_Values_And_Renders()
    {
        var cut = Render<Ex093_CustomComponentBaseLifecycle>(p => p.Add(c => c.Text, "hello"));

        Assert.Equal("hello", cut.Find("#text").TextContent);
        Assert.Equal("hello", cut.Instance.Text);
        Assert.Equal(1, cut.Instance.Renders);
    }

    [Fact]
    public void A_Later_Push_Applies_And_Renders_Again()
    {
        var cut = Render<Ex093_CustomComponentBaseLifecycle>(p => p.Add(c => c.Text, "hello"));

        cut.Render(p => p.Add(c => c.Text, "goodbye"));

        Assert.Equal("goodbye", cut.Find("#text").TextContent);
        Assert.Equal(2, cut.Instance.Renders);
    }

    [Fact]
    public void StateHasChanged_Renders_On_Its_Own()
    {
        var cut = Render<Ex093_CustomComponentBaseLifecycle>(p => p.Add(c => c.Text, "hello"));

        cut.InvokeAsync(() => cut.Instance.StateHasChanged());

        cut.WaitForAssertion(() => Assert.Equal(2, cut.Instance.Renders));
    }

    // Ruling: SetParameterProperties is not the same as assigning the properties by
    // hand. A push that carries no Text leaves the value where it was; a hand-rolled
    // "read it out of the ParameterView and assign" would null it.
    [Fact]
    public void A_Push_That_Omits_A_Parameter_Leaves_It_Alone()
    {
        var cut = Render<Ex093_CustomComponentBaseLifecycle>(p => p.Add(c => c.Text, "hello"));

        cut.Render(_ => { });

        Assert.Equal("hello", cut.Instance.Text);
        Assert.Equal("hello", cut.Find("#text").TextContent);
    }

    // Non-vacuity for the render handle: a component that kept it renders again on
    // request, over and over, rather than going quiet after the first pass.
    [Fact]
    public void The_Handle_Keeps_Working_For_Later_Requests()
    {
        var cut = Render<Ex093_CustomComponentBaseLifecycle>(p => p.Add(c => c.Text, "hello"));

        cut.InvokeAsync(() => cut.Instance.StateHasChanged());
        cut.WaitForAssertion(() => Assert.Equal(2, cut.Instance.Renders));

        cut.InvokeAsync(() => cut.Instance.StateHasChanged());
        cut.WaitForAssertion(() => Assert.Equal(3, cut.Instance.Renders));
    }
}
