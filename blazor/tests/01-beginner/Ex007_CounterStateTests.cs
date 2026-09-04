using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex007_CounterStateTests : BunitContext
{
    [Fact]
    public void Starts_At_The_Start_Parameter()
    {
        var cut = Render<Ex007_CounterState>(p => p.Add(c => c.Start, 3));

        Assert.Equal("3", cut.Find("#value").TextContent);
    }

    [Fact]
    public void One_Increment_Click_Adds_One()
    {
        var cut = Render<Ex007_CounterState>(p => p.Add(c => c.Start, 3));

        cut.Find("#inc").Click();

        cut.WaitForAssertion(() => Assert.Equal("4", cut.Find("#value").TextContent));
    }

    [Fact]
    public void One_Decrement_Click_Subtracts_One()
    {
        var cut = Render<Ex007_CounterState>(p => p.Add(c => c.Start, 3));

        cut.Find("#dec").Click();

        cut.WaitForAssertion(() => Assert.Equal("2", cut.Find("#value").TextContent));
    }

    [Fact]
    public void Three_Increment_Clicks_Accumulate()
    {
        var cut = Render<Ex007_CounterState>(p => p.Add(c => c.Start, 3));

        cut.Find("#inc").Click();
        cut.Find("#inc").Click();
        cut.Find("#inc").Click();

        cut.WaitForAssertion(() => Assert.Equal("6", cut.Find("#value").TextContent));
    }
}
