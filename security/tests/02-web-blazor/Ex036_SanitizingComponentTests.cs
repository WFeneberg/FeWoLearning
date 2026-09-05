using Bunit;
using FeWoLearning.Security.Exercises.WebBlazor;
using FeWoLearning.Security.Tests.Harness;
using TestContext = Xunit.TestContext;

namespace FeWoLearning.Security.Tests.WebBlazor;

public class Ex036_SanitizingComponentTests
{
    [Fact]
    public void Attack_A_Tag_Absent_From_AllowedTags_Is_Stripped_Even_If_Harmless_Looking()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex036_SanitizingComponent>(p => p
            .Add(c => c.Html, "<b>bold</b>")
            .Add(c => c.AllowedTags, new[] { "em" }));

        var html = cut.Find("#sanitized").InnerHtml;

        Assert.DoesNotContain("<b>", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<b ", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Attack_All_Attributes_Are_Stripped_From_An_Allowed_Tag()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex036_SanitizingComponent>(p => p
            .Add(c => c.Html, "<em onclick=\"evil()\" class=\"x\">click</em>")
            .Add(c => c.AllowedTags, new[] { "em" }));

        var html = cut.Find("#sanitized").InnerHtml;

        Assert.DoesNotContain("onclick", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("class=", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Attack_An_Unclosed_Tag_Fragment_Does_Not_Swallow_Following_Content()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex036_SanitizingComponent>(p => p
            .Add(c => c.Html, "before <em unclosed middle <strong>bold</strong> end")
            .Add(c => c.AllowedTags, new[] { "strong" }));

        var text = cut.Find("#sanitized").TextContent;

        Assert.Contains("before", text);
        Assert.Contains("middle", text);
        Assert.Contains("bold", text);
        Assert.Contains("end", text);
        Assert.Empty(cut.FindAll("em"));
    }

    [Fact]
    public void Use_Allowed_Tags_Survive_With_Their_Text_But_Without_Attributes()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex036_SanitizingComponent>(p => p
            .Add(c => c.Html, "<em onclick=\"evil()\">important</em>")
            .Add(c => c.AllowedTags, new[] { "em" }));

        Assert.Equal("<em>important</em>", cut.Find("#sanitized").InnerHtml);
    }

    [Fact]
    public void Use_An_Empty_AllowedTags_Still_Renders_The_Inputs_Text_Content()
    {
        using var harness = new BlazorHarness();

        var cut = harness.Render<Ex036_SanitizingComponent>(p => p
            .Add(c => c.Html, "hello <b>world</b>")
            .Add(c => c.AllowedTags, Array.Empty<string>()));

        Assert.Equal("hello world", cut.Find("#sanitized").TextContent);
    }
}
