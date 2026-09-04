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
    public void Splats_Unmatched_Attributes_Onto_The_Button()
    {
        var cut = Render<Ex014_AttributeSplatting>();

        // Pre-state sanity, folded in here rather than as a standalone fact: with
        // no unmatched attributes supplied, #btn must not carry a stray data-test
        // attribute. On its own this would pass the moment the button itself is
        // written, whether or not @attributes is ever wired up - only the
        // assertions below, after actually adding an unmatched attribute, exercise
        // the splatting TODO.
        Assert.False(cut.Find("#btn").HasAttribute("data-test"));

        cut.Render(p => p.AddUnmatched("data-test", "x").AddUnmatched("disabled", true));

        var button = cut.Find("#btn");
        Assert.Equal("x", button.GetAttribute("data-test"));
        Assert.True(button.HasAttribute("disabled"));
    }
}
