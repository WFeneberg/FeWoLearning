using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex027_CspNonceFlowTests
{
    [Fact]
    public void Attack_No_Inline_Event_Handler_Attributes_Are_Rendered()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex027_CspNonceFlow>(p => p.AddCascadingValue(new Ex027_CspNonce("n0nce-abc123")));

        Assert.DoesNotContain("onclick=", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror=", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Attack_No_Cascaded_Nonce_Renders_No_Script_Element_At_All()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex027_CspNonceFlow>();

        Assert.Empty(cut.FindAll("script"));
    }

    [Fact]
    public void Use_Cascaded_Nonce_Value_Is_Emitted_Verbatim()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex027_CspNonceFlow>(p => p.AddCascadingValue(new Ex027_CspNonce("n0nce-abc123")));

        var script = cut.Find("script#s");
        Assert.Equal("n0nce-abc123", script.GetAttribute("nonce"));
    }

    [Fact]
    public void Use_Script_Body_Still_Renders()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex027_CspNonceFlow>(p => p
            .AddCascadingValue(new Ex027_CspNonce("n0nce-abc123"))
            .Add(c => c.ScriptBody, "console.log('hello from ex027');"));

        Assert.Contains("console.log('hello from ex027')", cut.Find("script#s").TextContent);
    }
}
