using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex013_TemplatedFragmentTests : BunitContext
{
    [Fact]
    public void Renders_One_Row_Per_Item_Using_The_Row_Template_In_Order()
    {
        var cut = Render<Ex013_TemplatedFragment<string>>(p => p
            .Add(c => c.Items, new[] { "a", "b" })
            .Add(c => c.Row, item => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", "cell");
                builder.AddContent(2, item);
                builder.CloseElement();
            })
            .Add(c => c.Empty, "<p id=\"none\">none</p>"));

        Assert.Equal(2, cut.FindAll("#repeater .row").Count);
        var texts = cut.FindAll("#repeater .row .cell").Select(e => e.TextContent).ToArray();
        Assert.Equal(new[] { "a", "b" }, texts);

        // Items is non-empty here, so Empty must not render alongside the rows - this
        // is the only configuration that distinguishes "Empty instead of rows" from
        // "Empty as well as rows", which is the exercise's defining behaviour.
        Assert.Empty(cut.FindAll("#repeater #none"));
    }

    [Fact]
    public void Renders_The_Empty_Fragment_Instead_Of_Rows_When_Items_Is_Empty()
    {
        var cut = Render<Ex013_TemplatedFragment<string>>(p => p
            .Add(c => c.Items, Array.Empty<string>())
            .Add(c => c.Empty, "<p id=\"none\">none</p>"));

        Assert.Equal("none", cut.Find("#repeater #none").TextContent);
        Assert.Empty(cut.FindAll("#repeater .row"));
    }

    [Fact]
    public void Renders_An_Empty_Repeater_When_Items_Is_Empty_And_No_Empty_Fragment_Is_Supplied()
    {
        var cut = Render<Ex013_TemplatedFragment<string>>(p => p.Add(c => c.Items, Array.Empty<string>()));

        Assert.Equal("DIV", cut.Find("#repeater").TagName);
        Assert.Empty(cut.FindAll("#repeater .row"));
    }

    [Fact]
    public void Falls_Back_To_ToString_When_No_Row_Template_Is_Supplied()
    {
        var cut = Render<Ex013_TemplatedFragment<string>>(p => p.Add(c => c.Items, new[] { "a" }));

        Assert.Equal("a", cut.Find("#repeater .row").TextContent);
    }
}
