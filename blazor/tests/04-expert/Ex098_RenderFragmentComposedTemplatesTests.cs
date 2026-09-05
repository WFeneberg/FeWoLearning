using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

public class Ex098_RenderFragmentComposedTemplatesTests : BunitContext
{
    private static readonly Person[] People =
    [
        new(1, "Ada"), new(2, "Grace"), new(3, "Edsger"),
    ];

    [Fact]
    public void ForEach_Applies_The_Template_To_Every_Item_In_Order()
    {
        var cut = Render<Ex098_RenderFragmentComposedTemplates>(p => p.Add(c => c.People, People));

        var rows = cut.FindAll(".people .person");
        Assert.Equal(3, rows.Count);
        Assert.Equal("Ada", rows[0].TextContent);
        Assert.Equal("Edsger", rows[2].TextContent);
    }

    // Decorate wraps without touching the value; the LI is its doing, the text is
    // the inner template's.
    [Fact]
    public void Decorate_Wraps_Each_Item_Without_Changing_It()
    {
        var cut = Render<Ex098_RenderFragmentComposedTemplates>(p => p.Add(c => c.People, People));

        var row = cut.FindAll(".people .person")[0];
        Assert.Equal("LI", row.TagName);
        Assert.Equal("Ada", row.TextContent);
    }

    // Ruling: Adapt is what lets a template over strings render a Person. Without the
    // mapping the inner template would be handed the Person itself, and the rendered
    // text would be the record's ToString rather than the name.
    [Fact]
    public void Adapt_Maps_The_Value_Before_The_Inner_Template_Sees_It()
    {
        var cut = Render<Ex098_RenderFragmentComposedTemplates>(p => p.Add(
            c => c.People, new[] { new Person(7, "Barbara") }));

        Assert.Equal("Barbara", cut.Find(".people .person").TextContent);
    }

    // Negative assertion, so it stays bare: an empty sequence renders no rows, and
    // the surrounding list is still there.
    [Fact]
    public void ForEach_Over_Nothing_Renders_No_Rows()
    {
        var cut = Render<Ex098_RenderFragmentComposedTemplates>(p => p.Add(
            c => c.People, Array.Empty<Person>()));

        Assert.Empty(cut.FindAll(".people .person"));
        Assert.Empty(cut.Find(".people").Children);
    }

    // The helpers are ordinary functions, so they compose outside the component too -
    // which is the whole reason for writing them this way.
    [Fact]
    public void The_Helpers_Compose_Standalone()
    {
        var template = Ex098_RenderFragmentComposedTemplates.Decorate<int>(
            "b",
            "doubled",
            Ex098_RenderFragmentComposedTemplates.Adapt<int, string>(
                value => (value * 2).ToString(),
                Ex098_RenderFragmentComposedTemplates.Label()));

        var cut = Render(Ex098_RenderFragmentComposedTemplates.ForEach([1, 2, 3], template));

        var doubled = cut.FindAll(".doubled");
        Assert.Equal(3, doubled.Count);
        Assert.Equal("2", doubled[0].TextContent);
        Assert.Equal("6", doubled[2].TextContent);
    }
}
