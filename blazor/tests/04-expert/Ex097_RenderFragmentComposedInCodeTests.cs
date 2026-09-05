using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex097_RenderFragmentComposedInCodeTests : BunitContext
{
    // Render(RenderFragment) hands back bUnit's fragment result; `var` because the
    // type name is not needed anywhere here.
    [Fact]
    public void Text_Renders_Its_Value_Escaped()
    {
        var cut = Render(
            Ex097_RenderFragmentComposedInCode.Wrap(
                "div", "frame", Ex097_RenderFragmentComposedInCode.Text("<b>hi</b>")));

        var frame = cut.Find(".frame");
        Assert.Equal("<b>hi</b>", frame.TextContent);
        Assert.Empty(frame.Children);
    }

    [Fact]
    public void Concat_Renders_Its_Parts_In_Order()
    {
        var cut = Render(
            Ex097_RenderFragmentComposedInCode.Wrap(
                "div",
                "frame",
                Ex097_RenderFragmentComposedInCode.Concat(
                    Ex097_RenderFragmentComposedInCode.Wrap("span", "item", Ex097_RenderFragmentComposedInCode.Text("one")),
                    Ex097_RenderFragmentComposedInCode.Wrap("span", "item", Ex097_RenderFragmentComposedInCode.Text("two")))));

        var items = cut.FindAll(".frame .item");
        Assert.Equal(2, items.Count);
        Assert.Equal("one", items[0].TextContent);
        Assert.Equal("two", items[1].TextContent);
    }

    // Nothing at all, rather than an empty element - the distinction that separates a
    // composed fragment from a container. Negative assertion, so it stays bare.
    [Fact]
    public void Concat_Of_Nothing_Renders_Nothing()
    {
        var cut = Render(Ex097_RenderFragmentComposedInCode.Concat());

        Assert.Empty(cut.FindAll("*"));
    }

    [Fact]
    public void Wrap_Puts_The_Inner_Fragment_Inside_Its_Element()
    {
        var cut = Render(
            Ex097_RenderFragmentComposedInCode.Wrap(
                "section", "frame", Ex097_RenderFragmentComposedInCode.Text("inside")));

        var frame = cut.Find(".frame");
        Assert.Equal("SECTION", frame.TagName);
        Assert.Equal("inside", frame.TextContent);
    }

    [Fact]
    public void The_Component_Composes_All_Three()
    {
        var cut = Render<Ex097_RenderFragmentComposedInCode>(p => p.Add(
            c => c.Items, new[] { "alpha", "beta", "gamma" }));

        var items = cut.FindAll(".frame .item");
        Assert.Equal(3, items.Count);
        Assert.Equal("alpha", items[0].TextContent);
        Assert.Equal("gamma", items[2].TextContent);
    }

    [Fact]
    public void An_Empty_Item_List_Leaves_An_Empty_Frame()
    {
        var cut = Render<Ex097_RenderFragmentComposedInCode>(p => p.Add(
            c => c.Items, Array.Empty<string>()));

        Assert.Empty(cut.Find(".frame").Children);
    }
}
