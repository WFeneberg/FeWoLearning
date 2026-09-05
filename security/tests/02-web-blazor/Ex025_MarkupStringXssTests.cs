using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex025_MarkupStringXssTests
{
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe>")]
    public void Attack_Injection_Never_Reaches_The_Rendered_Markup(string payload)
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(p => p.Add(c => c.Body, payload));
        var html = cut.Find("#comment").InnerHtml;

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("javascript:", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<iframe", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Allowlisted_Formatting_Survives()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(
            p => p.Add(c => c.Body, "an <em>important</em> and <strong>bold</strong> point"));
        var html = cut.Find("#comment").InnerHtml;

        Assert.Contains("<em>important</em>", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<strong>bold</strong>", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Use_Plain_Text_Renders_Unchanged()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(p => p.Add(c => c.Body, "just a comment"));

        Assert.Equal("just a comment", cut.Find("#comment").TextContent);
    }
}
