using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex023_InputTextBindingTests : BunitContext
{
    [Fact]
    public void Starts_Empty()
    {
        var cut = Render<Ex023_InputTextBinding>();

        Assert.Equal("", cut.Find("#echo").TextContent);
        Assert.Equal("0", cut.Find("#len").TextContent);
    }

    [Fact]
    public void Changing_The_Input_Updates_The_Echo_And_Length()
    {
        var cut = Render<Ex023_InputTextBinding>();

        cut.Find("#note").Change("hi");

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("hi", cut.Find("#echo").TextContent);
            Assert.Equal("2", cut.Find("#len").TextContent);
        });
    }

    [Fact]
    public void Clearing_The_Input_After_A_Value_Resets_The_Echo_And_Length()
    {
        var cut = Render<Ex023_InputTextBinding>();
        cut.Find("#note").Change("hi");
        cut.WaitForAssertion(() => Assert.Equal("hi", cut.Find("#echo").TextContent));

        cut.Find("#note").Change("");

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("", cut.Find("#echo").TextContent);
            Assert.Equal("0", cut.Find("#len").TextContent);
        });
    }
}
