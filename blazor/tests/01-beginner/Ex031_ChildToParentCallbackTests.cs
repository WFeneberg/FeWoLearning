using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex031_ChildToParentCallbackTests : BunitContext
{
    [Fact]
    public void Initially_Total_Is_Zero_And_Three_Add_Buttons_Exist()
    {
        var cut = Render<Ex031_ChildToParentCallback>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("0", cut.Find("#total").TextContent);
            var buttons = cut.FindAll("button.add");
            Assert.Equal(3, buttons.Count);
            Assert.Equal(new[] { "1", "5", "10" }, buttons.Select(b => b.GetAttribute("data-amount")).ToArray());
        });
    }

    [Fact]
    public void Clicking_The_Plus_Five_Button_Adds_Five()
    {
        var cut = Render<Ex031_ChildToParentCallback>();

        cut.Find("button.add[data-amount='5']").Click();

        cut.WaitForAssertion(() => Assert.Equal("5", cut.Find("#total").TextContent));
    }

    [Fact]
    public void Clicking_Plus_Five_Plus_Ten_Plus_One_Accumulates_To_Sixteen()
    {
        var cut = Render<Ex031_ChildToParentCallback>();

        cut.Find("button.add[data-amount='5']").Click();
        cut.Find("button.add[data-amount='10']").Click();
        cut.Find("button.add[data-amount='1']").Click();

        cut.WaitForAssertion(() => Assert.Equal("16", cut.Find("#total").TextContent));
    }
}
