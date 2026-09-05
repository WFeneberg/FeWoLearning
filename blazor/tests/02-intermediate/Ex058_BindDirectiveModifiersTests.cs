using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex058_BindDirectiveModifiersTests : BunitContext
{
    // Input() raises oninput, which plain @bind (onchange) ignores - so this fact is
    // exactly the @bind:event="oninput" modifier and nothing else.
    [Fact]
    public void Note_Updates_On_Every_Keystroke_Not_Only_On_Change()
    {
        var cut = Render<Ex058_BindDirectiveModifiers>();

        cut.Find("#note").Input("hi");

        cut.WaitForAssertion(() => Assert.Equal("hi", cut.Find("#echo").TextContent));
    }

    // @bind:after runs once per write, so the keystroke-level trigger above means one
    // hook call per Input(). A hook wired to onchange instead would still read 0 here.
    [Fact]
    public void After_Hook_Runs_Once_Per_Write()
    {
        var cut = Render<Ex058_BindDirectiveModifiers>();

        cut.Find("#note").Input("h");
        cut.Find("#note").Input("hi");

        cut.WaitForAssertion(() => Assert.Equal("2", cut.Find("#edits").TextContent));
    }

    // Without @bind:format the DateTime round-trips as "2026-01-15T00:00:00".
    // Note: @bind:format formats with CultureInfo.CurrentCulture, so this fact assumes
    // a Gregorian-calendar current culture; ex009 is the exercise about invariance.
    [Fact]
    public void Due_Renders_Through_The_Bound_Format()
    {
        var cut = Render<Ex058_BindDirectiveModifiers>();

        Assert.Equal("2026-01-15", cut.Find("#due").GetAttribute("value"));
    }

    [Fact]
    public void Due_Parses_Back_Through_The_Bound_Format()
    {
        var cut = Render<Ex058_BindDirectiveModifiers>();

        cut.Find("#due").Change("2026-03-04");

        cut.WaitForAssertion(() => Assert.Equal("2026-03-04", cut.Find("#due-echo").TextContent));
        Assert.Equal("2026-03-04", cut.Find("#due").GetAttribute("value"));
    }

    // Non-vacuity for the after-hook: it must not fire on the initial render, only on
    // a write. Negative assertion, so it stays bare (README section 11).
    [Fact]
    public void After_Hook_Does_Not_Run_On_The_First_Render()
    {
        var cut = Render<Ex058_BindDirectiveModifiers>();

        Assert.Equal("0", cut.Find("#edits").TextContent);
    }
}
