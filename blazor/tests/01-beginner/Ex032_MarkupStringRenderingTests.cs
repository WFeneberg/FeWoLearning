using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex032_MarkupStringRenderingTests : BunitContext
{
    [Fact]
    public void Disallowed_Html_Is_Escaped_As_Text()
    {
        var cut = Render<Ex032_MarkupStringRendering>(p => p
            .Add(c => c.Html, "<b>hi</b>")
            .Add(c => c.AllowHtml, false));

        Assert.Empty(cut.FindAll("#rich b"));
        Assert.Equal("<b>hi</b>", cut.Find("#rich").TextContent);
    }

    [Fact]
    public void Allowed_Html_Is_Rendered_As_Markup()
    {
        var cut = Render<Ex032_MarkupStringRendering>(p => p
            .Add(c => c.Html, "<b>hi</b>")
            .Add(c => c.AllowHtml, true));

        Assert.Equal("hi", cut.Find("#rich b").TextContent);
    }

    [Fact]
    public void Empty_Html_With_AllowHtml_Renders_An_Empty_Rich_Element()
    {
        var cut = Render<Ex032_MarkupStringRendering>(p => p
            .Add(c => c.Html, "")
            .Add(c => c.AllowHtml, true));

        var rich = cut.Find("#rich");
        Assert.Equal("DIV", rich.TagName);
        Assert.Equal("", rich.TextContent);
    }
}
