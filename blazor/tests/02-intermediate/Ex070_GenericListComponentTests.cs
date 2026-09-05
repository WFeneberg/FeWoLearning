using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex070_GenericListComponentTests : BunitContext
{
    [Fact]
    public void Renders_One_Row_Per_Item_In_Order()
    {
        var cut = Render<Ex070_GenericListComponent<string>>(p => p.Add(
            c => c.Items, new[] { "pear", "apple", "quince" }));

        var rows = cut.FindAll("#list li");
        Assert.Equal(3, rows.Count);
        Assert.Equal("pear", rows[0].TextContent);
        Assert.Equal("apple", rows[1].TextContent);
        Assert.Equal("quince", rows[2].TextContent);
    }

    // A different TItem, and a template that does arithmetic on the item - which only
    // compiles because the fragment's context is an int rather than an object.
    [Fact]
    public void Uses_The_Item_Template_When_One_Is_Given()
    {
        RenderFragment<int> doubled = item => builder => builder.AddMarkupContent(0, $"<b>{item * 2}</b>");

        var cut = Render<Ex070_GenericListComponent<int>>(p => p
            .Add(c => c.Items, new[] { 1, 2, 3 })
            .Add(c => c.ItemTemplate, doubled));

        var rows = cut.FindAll("#list li b");
        Assert.Equal(3, rows.Count);
        Assert.Equal("2", rows[0].TextContent);
        Assert.Equal("6", rows[2].TextContent);
    }

    [Fact]
    public void An_Empty_List_Renders_The_Empty_Template_Instead_Of_A_List()
    {
        RenderFragment empty = builder => builder.AddMarkupContent(0, "<span id=\"empty\">nothing here</span>");

        var cut = Render<Ex070_GenericListComponent<string>>(p => p
            .Add(c => c.Items, Array.Empty<string>())
            .Add(c => c.EmptyTemplate, empty));

        Assert.Equal("nothing here", cut.Find("#empty").TextContent);
        Assert.Empty(cut.FindAll("#list"));
    }

    // Not a markup-string comparison (README §11) - an element count of zero is the
    // assertion, and it is what separates "renders nothing" from "renders an empty
    // <ul>", which a foreach over an empty list would produce.
    [Fact]
    public void An_Empty_List_With_No_Empty_Template_Renders_Nothing()
    {
        var cut = Render<Ex070_GenericListComponent<string>>(p => p.Add(
            c => c.Items, Array.Empty<string>()));

        Assert.Empty(cut.FindAll("*"));
    }
}
