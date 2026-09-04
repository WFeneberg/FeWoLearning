using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex010_BindEventOnInputTests : BunitContext
{
    [Fact]
    public void Renders_The_Query_In_The_Input_And_The_Echo()
    {
        var cut = Render<Ex010_BindEventOnInput>(p => p.Add(c => c.Query, "ab"));

        Assert.Equal("ab", cut.Find("#q").GetAttribute("value"));
        Assert.Equal("ab", cut.Find("#echo").TextContent);
    }

    [Fact]
    public void Input_Reports_On_Every_Keystroke()
    {
        var current = "ab";
        var cut = Render<Ex010_BindEventOnInput>(p => p.Bind(c => c.Query, current, v => current = v));

        cut.Find("#q").Input("abc");

        Assert.Equal("abc", current);
    }

    [Fact]
    public void Change_Alone_Is_Not_Wired_Because_Only_Oninput_Owns_The_Reporting()
    {
        var current = "ab";
        var cut = Render<Ex010_BindEventOnInput>(p => p.Bind(c => c.Query, current, v => current = v));

        // The element has no onchange handler at all when only @oninput is wired, so
        // bUnit rejects the dispatch outright - that absence is itself the proof.
        Assert.Throws<MissingEventHandlerException>(() => cut.Find("#q").Change("xyz"));
        Assert.Equal("ab", current);
    }
}
