using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex021_EventArgsHandlingTests : BunitContext
{
    [Fact]
    public void A_Letter_Key_Is_Recorded_Upper_Cased()
    {
        var cut = Render<Ex021_EventArgsHandling>();

        cut.Find("#k").KeyDown("a");

        cut.WaitForAssertion(() => Assert.Equal("A", cut.Find("#last").TextContent));
    }

    [Fact]
    public void The_Enter_Key_Is_Recorded_Upper_Cased()
    {
        var cut = Render<Ex021_EventArgsHandling>();

        cut.Find("#k").KeyDown("Enter");

        cut.WaitForAssertion(() => Assert.Equal("ENTER", cut.Find("#last").TextContent));
    }

    [Fact]
    public void A_Modifier_Key_After_A_Letter_Does_Not_Overwrite_The_Last_Key()
    {
        var cut = Render<Ex021_EventArgsHandling>();

        cut.Find("#k").KeyDown("a");
        cut.Find("#k").KeyDown("Shift");

        cut.WaitForAssertion(() => Assert.Equal("A", cut.Find("#last").TextContent));
    }

    [Fact]
    public void A_Modifier_Key_As_The_First_Event_Leaves_Last_Empty()
    {
        var cut = Render<Ex021_EventArgsHandling>();

        cut.Find("#k").KeyDown("Shift");

        cut.WaitForAssertion(() => Assert.Equal("", cut.Find("#last").TextContent));
    }
}
