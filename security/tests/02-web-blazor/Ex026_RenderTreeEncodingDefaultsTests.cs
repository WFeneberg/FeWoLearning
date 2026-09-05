using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex026_RenderTreeEncodingDefaultsTests
{
    [Fact]
    public void Attack_Script_In_Text_Produces_No_Script_Element()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex026_RenderTreeEncodingDefaults>(p => p
            .Add(c => c.Text, "<script>alert(1)</script>")
            .Add(c => c.CssClass, "note"));

        Assert.Empty(cut.FindAll("script"));
        Assert.DoesNotContain("<script", cut.Find("#out").InnerHtml, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Attack_Attribute_Break_In_CssClass_Produces_No_Onmouseover_Attribute()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex026_RenderTreeEncodingDefaults>(p => p
            .Add(c => c.Text, "note")
            .Add(c => c.CssClass, "x\" onmouseover=\"alert(1)"));

        var span = cut.Find("#out");
        Assert.Null(span.GetAttribute("onmouseover"));
        Assert.DoesNotContain("onmouseover=\"alert", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("MarkupString", true)]
    [InlineData("text", false)]
    [InlineData("attribute", false)]
    public void Use_RequiresManualEncoding_Reports_Each_Sink_Correctly(string sink, bool expected)
    {
        Assert.Equal(expected, Ex026_RenderTreeEncodingDefaults.RequiresManualEncoding(sink));
    }

    [Fact]
    public void Use_Plain_Text_And_Class_Render_Correctly()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex026_RenderTreeEncodingDefaults>(p => p
            .Add(c => c.Text, "hello")
            .Add(c => c.CssClass, "highlight"));

        var span = cut.Find("#out");
        Assert.Equal("hello", span.TextContent);
        Assert.Equal("highlight", span.GetAttribute("class"));
    }
}
