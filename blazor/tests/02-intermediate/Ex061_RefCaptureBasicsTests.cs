using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex061_RefCaptureBasicsTests : BunitContext
{
    [Fact]
    public void Panel_Starts_Closed()
    {
        var cut = Render<Ex061_RefCaptureBasics>();

        Assert.Equal("closed", cut.Find("#panel").TextContent);
    }

    // The parent has no other way to reach the child: no parameter, no cascade, no
    // event. Only the captured instance can flip it.
    [Fact]
    public void Clicking_Open_Drives_The_Child_Through_The_Captured_Instance()
    {
        var cut = Render<Ex061_RefCaptureBasics>();

        cut.Find("#open").Click();

        cut.WaitForAssertion(() => Assert.Equal("open", cut.Find("#panel").TextContent));
    }

    // Non-vacuity for the component ref: it must be the child that is actually on
    // screen, not a second instance the parent constructed for itself - so this
    // compares identity against the one bUnit finds in the render tree (README §8).
    [Fact]
    public void Component_Ref_Is_The_Rendered_Child_Instance()
    {
        var cut = Render<Ex061_RefCaptureBasics>();

        Assert.Same(cut.FindComponent<Ex061_RefCaptureBasics_Panel>().Instance, cut.Instance.Panel);
    }

    // What bUnit can prove about an element ref is that the framework assigned one -
    // the Id is an opaque handle, and there is no browser here to resolve it against
    // (README §7). Without @ref the struct stays default and the Id is null.
    [Fact]
    public void Element_Ref_Is_Populated_After_The_Render()
    {
        var cut = Render<Ex061_RefCaptureBasics>();

        Assert.False(string.IsNullOrEmpty(cut.Instance.ElementId));
    }
}
