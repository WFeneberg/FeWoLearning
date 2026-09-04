using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex006_ClickEventCallbackTests : BunitContext
{
    [Fact]
    public void One_Click_Reports_Count_Plus_One_Exactly_Once()
    {
        // Renders the label from the given, non-TODO markup: not asserted on its own
        // (that would pass against the untouched stub and prove nothing), only as a
        // sanity check on the way to the click that actually exercises the TODO.
        var received = new List<int>();
        var cut = Render<Ex006_ClickEventCallback>(p => p
            .Add(c => c.Count, 4)
            .Add(c => c.OnLike, EventCallback.Factory.Create<int>(this, v => received.Add(v))));

        Assert.Equal("Like (4)", cut.Find("#like").TextContent);

        cut.Find("#like").Click();

        Assert.Equal(new[] { 5 }, received);
    }

    [Fact]
    public void Two_Clicks_Report_The_Same_Next_Value_Twice_Because_The_Component_Does_Not_Accumulate()
    {
        var received = new List<int>();
        var cut = Render<Ex006_ClickEventCallback>(p => p
            .Add(c => c.Count, 4)
            .Add(c => c.OnLike, EventCallback.Factory.Create<int>(this, v => received.Add(v))));

        cut.Find("#like").Click();
        cut.Find("#like").Click();

        Assert.Equal(new[] { 5, 5 }, received);
    }
}
