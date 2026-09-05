using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex025_MarkupStringXssTests
{
    // The four tags Sanitize is allowed to emit. Anything else that survives into
    // the rendered DOM - and any attribute at all, on any tag - is a finding.
    private static readonly string[] AllowedTags = ["em", "strong", "b", "i"];

    // The first four payloads are the textbook ones every denylist already knows.
    // The last three are the reason this theory exists in its present shape: each
    // slips past a complete-enough denylist (an unknown element with an inline
    // handler; a space between the handler name and its "="; a scheme hidden
    // behind an HTML entity), and each is stopped by an allowlist without the
    // allowlist having to know anything about it. Substring assertions alone
    // cannot express that - an entity-encoded "javascript:" contains no literal
    // "javascript:" - so the check below is on the parsed DOM instead.
    [Theory]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("<a href=\"javascript:alert(1)\">click</a>")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe>")]
    [InlineData("<svg/onload=alert(1)>")]
    [InlineData("<img src=x onerror =alert(1)>")]
    [InlineData("<a href=\"&#106;avascript:alert(1)\">click</a>")]
    public void Attack_Injection_Never_Reaches_The_Rendered_Markup(string payload)
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex025_MarkupStringXss>(p => p.Add(c => c.Body, payload));
        var comment = cut.Find("#comment");
        var html = comment.InnerHtml;

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("javascript:", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<iframe", html, StringComparison.OrdinalIgnoreCase);

        foreach (var element in comment.QuerySelectorAll("*"))
        {
            Assert.True(
                AllowedTags.Contains(element.LocalName, StringComparer.OrdinalIgnoreCase),
                $"<{element.LocalName}> survived sanitisation of: {payload}");
            Assert.True(
                element.Attributes.Length == 0,
                $"<{element.LocalName}> kept attributes after sanitisation of: {payload}");
        }
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
