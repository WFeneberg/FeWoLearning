using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

public class Ex086_RenderModeStaticSsrTests : BunitContext
{
    public Ex086_RenderModeStaticSsrTests()
        => JSInterop.SetupVoid(Ex086_RenderModeStaticSsr.SetupFunction);

    private int SetupCalls => JSInterop.Invocations[Ex086_RenderModeStaticSsr.SetupFunction].Count;

    [Fact]
    public void A_Static_Render_Falls_Back_To_A_Real_Form()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        var cut = Render<Ex086_RenderModeStaticSsr>();

        Assert.Equal("static", cut.Find("#mode").TextContent);
        Assert.Equal("post", cut.Find("#fallback").GetAttribute("method"));
        Assert.Empty(cut.FindAll("#save"));
    }

    [Fact]
    public void An_Interactive_Render_Uses_The_Click_Handler_Instead()
    {
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));

        var cut = Render<Ex086_RenderModeStaticSsr>();

        Assert.Equal("interactive", cut.Find("#mode").TextContent);
        Assert.Empty(cut.FindAll("#fallback"));

        cut.Find("#save").Click();

        Assert.Equal(1, cut.Instance.Saves);
    }

    // Ruling: bUnit runs OnAfterRenderAsync whatever RendererInfo says, so nothing
    // but the component's own guard prevents this call. In a real static render the
    // interop has no channel and throws - which is the failure this fact stands in
    // for. Negative assertion, so it stays bare (README §11).
    [Fact]
    public void A_Static_Render_Makes_No_Interop_Call()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        Render<Ex086_RenderModeStaticSsr>();

        Assert.Equal(0, SetupCalls);
    }

    [Fact]
    public void An_Interactive_Render_Sets_Up_Once()
    {
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));

        var cut = Render<Ex086_RenderModeStaticSsr>();
        cut.Render();

        Assert.Equal(1, SetupCalls);
    }
}
