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

        // Folded here per rule 1 (identical premise: AllowHtml=true): an empty Html
        // value can't discriminate escaped-vs-markup behaviour on its own - a
        // MarkupString("") and an escaped "" both render as empty text - so this
        // isn't a fact worth keeping separate. It only confirms the empty case
        // still renders the given #rich element without throwing.
        cut.Render(p => p.Add(c => c.Html, "").Add(c => c.AllowHtml, true));
        var rich = cut.Find("#rich");
        Assert.Equal("DIV", rich.TagName);
        Assert.Equal("", rich.TextContent);
    }
}
