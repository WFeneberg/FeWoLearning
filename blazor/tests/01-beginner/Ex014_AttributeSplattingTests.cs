using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex014_AttributeSplattingTests : BunitContext
{
    [Fact]
    public void Renders_The_Label_Inside_The_Button()
    {
        var cut = Render<Ex014_AttributeSplatting>(p => p.Add(c => c.Label, "Go"));

        Assert.Equal("Go", cut.Find("#btn").TextContent);
    }

    [Fact]
    public void Splats_Unmatched_Attributes_Onto_The_Button_And_Lets_A_Caller_Supplied_Id_Win()
    {
        var cut = Render<Ex014_AttributeSplatting>(p => p.Add(c => c.Label, "Go"));

        // Folded in here rather than as a standalone fact: its premise (no unmatched
        // attributes supplied) is identical to this test's own starting state, so
        // the assertion is merged into this test instead of duplicating that setup.
        Assert.False(cut.Find("#btn").HasAttribute("data-test"));

        cut.Render(p => p.AddUnmatched("data-test", "x").AddUnmatched("disabled", true));

        var button = cut.Find("#btn");
        Assert.Equal("x", button.GetAttribute("data-test"));
        Assert.True(button.HasAttribute("disabled"));

        // A caller-supplied id must override the button's own default id="btn" -
        // proving @attributes is applied after id, not before (splat-order
        // precedence, the point the exercise's Drills line advertises).
        cut.Render(p => p.AddUnmatched("id", "custom"));
        Assert.Equal("Go", cut.Find("#custom").TextContent);
        Assert.Empty(cut.FindAll("#btn"));
    }
}
