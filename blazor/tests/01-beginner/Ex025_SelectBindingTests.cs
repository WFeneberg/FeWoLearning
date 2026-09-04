using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex025_SelectBindingTests : BunitContext
{
    [Fact]
    public void Renders_One_Option_Per_Entry_In_Order_With_Matching_Values()
    {
        var cut = Render<Ex025_SelectBinding>(p => p.Add(c => c.Options, new[] { "Low", "Normal", "High" }));

        var options = cut.FindAll("#prio option");

        Assert.Equal(3, options.Count);
        Assert.Equal(new[] { "Low", "Normal", "High" }, options.Select(o => o.TextContent).ToArray());
        Assert.Equal(new[] { "Low", "Normal", "High" }, options.Select(o => o.GetAttribute("value")).ToArray());

        // A different Options set must produce a different projection - this is
        // what actually rules out a hard-coded <option> list on its own, rather
        // than relying solely on the empty-list fact below to catch it.
        cut.Render(p => p.Add(c => c.Options, new[] { "X", "Y" }));

        var updated = cut.FindAll("#prio option");
        Assert.Equal(new[] { "X", "Y" }, updated.Select(o => o.TextContent).ToArray());
        Assert.Equal(new[] { "X", "Y" }, updated.Select(o => o.GetAttribute("value")).ToArray());
    }

    [Fact]
    public void Changing_The_Select_Updates_The_Chosen_Span()
    {
        var cut = Render<Ex025_SelectBinding>(p => p.Add(c => c.Options, new[] { "Low", "Normal", "High" }));

        cut.Find("#prio").Change("High");

        cut.WaitForAssertion(() => Assert.Equal("High", cut.Find("#chosen").TextContent));
    }

    [Fact]
    public void An_Empty_Options_List_Still_Renders_The_Select_With_No_Options()
    {
        var cut = Render<Ex025_SelectBinding>(p => p.Add(c => c.Options, Array.Empty<string>()));

        Assert.Equal("SELECT", cut.Find("#prio").TagName);
        Assert.Empty(cut.FindAll("#prio option"));
    }
}
