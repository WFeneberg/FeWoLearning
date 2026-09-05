using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex091_RenderTreeBuilderManualTests : BunitContext
{
    [Fact]
    public void Builds_The_Element_With_Its_Fixed_Attributes_And_Content()
    {
        var cut = Render<Ex091_RenderTreeBuilderManual>(p => p.Add(c => c.Label, "beta"));

        var chip = cut.Find("#chip");
        Assert.Equal("A", chip.TagName);
        Assert.Equal("chip", chip.GetAttribute("class"));
        Assert.Equal("beta", chip.TextContent);
    }

    [Fact]
    public void An_Href_That_Was_Given_Is_Rendered()
    {
        var cut = Render<Ex091_RenderTreeBuilderManual>(p => p
            .Add(c => c.Label, "beta")
            .Add(c => c.Href, "/somewhere"));

        Assert.Equal("/somewhere", cut.Find("#chip").GetAttribute("href"));
    }

    // The renderer drops a null attribute value itself, which is why the solution
    // needs no condition around it. Negative assertion, so it stays bare.
    [Fact]
    public void A_Null_Href_Produces_No_Attribute_At_All()
    {
        var cut = Render<Ex091_RenderTreeBuilderManual>(p => p.Add(c => c.Label, "beta"));

        Assert.False(cut.Find("#chip").HasAttribute("href"));
    }

    // Same rule for false, which is what makes bool-valued attributes work.
    [Fact]
    public void Disabled_Governs_Whether_The_Aria_Attribute_Exists()
    {
        var enabled = Render<Ex091_RenderTreeBuilderManual>(p => p.Add(c => c.Label, "beta"));
        Assert.False(enabled.Find("#chip").HasAttribute("aria-disabled"));

        var disabled = Render<Ex091_RenderTreeBuilderManual>(p => p
            .Add(c => c.Label, "beta")
            .Add(c => c.Disabled, true));

        Assert.True(disabled.Find("#chip").HasAttribute("aria-disabled"));
    }

    // Ruling: AddContent escapes, AddMarkupContent does not - and the difference is
    // invisible until someone's data contains a tag. An implementation reaching for
    // AddMarkupContent renders a real <b> here and fails.
    [Fact]
    public void The_Label_Is_Content_Not_Markup()
    {
        var cut = Render<Ex091_RenderTreeBuilderManual>(p => p.Add(c => c.Label, "<b>hi</b>"));

        var chip = cut.Find("#chip");
        Assert.Equal("<b>hi</b>", chip.TextContent);
        Assert.Empty(chip.Children);
    }
}
