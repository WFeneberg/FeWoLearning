using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

// Both knobs are bUnit's: SetRendererInfo says what kind of renderer the component
// believes it is running under, and SetAssignedRenderMode says which mode the host
// assigned it. Neither is inferred - a component that reads RendererInfo without one
// being set throws MissingRendererInfoException.
public class Ex085_RenderModeInteractiveServerTests : BunitContext
{
    [Fact]
    public void With_No_Assigned_Mode_It_Reports_None()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        var cut = Render<Ex085_RenderModeInteractiveServer>();

        Assert.Equal("none", cut.Find("#mode").TextContent);
    }

    [Fact]
    public void It_Reports_The_Interactive_Server_Mode_It_Was_Assigned()
    {
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));

        var cut = Render<Ex085_RenderModeInteractiveServer>(
            p => p.SetAssignedRenderMode(RenderMode.InteractiveServer));

        Assert.Equal("InteractiveServer", cut.Find("#mode").TextContent);
        Assert.Equal("Server", cut.Find("#renderer").TextContent);
    }

    // Non-vacuity: a member that just returns "InteractiveServer" passes the fact
    // above and fails this one.
    [Fact]
    public void A_Different_Mode_Is_Reported_Differently()
    {
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));

        var cut = Render<Ex085_RenderModeInteractiveServer>(
            p => p.SetAssignedRenderMode(RenderMode.InteractiveWebAssembly));

        Assert.Equal("InteractiveWebAssembly", cut.Find("#mode").TextContent);
    }

    // The prerender pattern: the markup exists, the interaction does not work yet, so
    // the button says so rather than silently swallowing clicks.
    [Fact]
    public void A_Static_Render_Reports_Static_And_Disables_The_Button()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        var cut = Render<Ex085_RenderModeInteractiveServer>();

        Assert.Equal("static", cut.Find("#interactive").TextContent);
        Assert.True(cut.Find("#act").HasAttribute("disabled"));
    }

    [Fact]
    public void An_Interactive_Render_Enables_It_And_The_Click_Lands()
    {
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));

        var cut = Render<Ex085_RenderModeInteractiveServer>(
            p => p.SetAssignedRenderMode(RenderMode.InteractiveServer));

        Assert.Equal("interactive", cut.Find("#interactive").TextContent);
        Assert.False(cut.Find("#act").HasAttribute("disabled"));

        cut.Find("#act").Click();

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#clicks").TextContent));
    }
}
