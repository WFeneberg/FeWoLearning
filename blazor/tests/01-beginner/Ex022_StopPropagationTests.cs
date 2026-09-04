using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex022_StopPropagationTests : BunitContext
{
    [Fact]
    public void Clicking_The_Inner_Button_Increments_Only_InnerClicks()
    {
        var cut = Render<Ex022_StopPropagation>();

        cut.Find("#inner").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, cut.Instance.InnerClicks);
            Assert.Equal(0, cut.Instance.OuterClicks);
        });
    }

    [Fact]
    public void Clicking_The_Outer_Div_Increments_Only_OuterClicks()
    {
        var cut = Render<Ex022_StopPropagation>();

        cut.Find("#outer").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, cut.Instance.OuterClicks);
            Assert.Equal(0, cut.Instance.InnerClicks);
        });
    }

    [Fact]
    public void The_Counts_Span_Reflects_Both_Counters_After_An_Inner_Click()
    {
        var cut = Render<Ex022_StopPropagation>();

        cut.Find("#inner").Click();

        cut.WaitForAssertion(() => Assert.Equal("0/1", cut.Find("#counts").TextContent));
    }
}
