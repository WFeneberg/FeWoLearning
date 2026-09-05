using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex066_ErrorBoundaryBasicsTests : BunitContext
{
    [Fact]
    public void Renders_The_Child_While_It_Behaves()
    {
        var cut = Render<Ex066_ErrorBoundaryBasics>(p => p.Add(c => c.Explode, false));

        Assert.Equal("ok", cut.Find("#child").TextContent);
        Assert.Empty(cut.FindAll("#error"));
    }

    // Without an ErrorContent fragment the boundary rethrows and Render itself fails,
    // so this fact covers both halves: the exception was caught, and the replacement
    // markup was given.
    [Fact]
    public void A_Throwing_Child_Is_Replaced_By_The_Error_Content()
    {
        var cut = Render<Ex066_ErrorBoundaryBasics>(p => p.Add(c => c.Explode, false));

        cut.Render(p => p.Add(c => c.Explode, true));

        Assert.Equal(ExplodingChild.Message, cut.Find("#error").TextContent);
        Assert.Empty(cut.FindAll("#child"));
    }

    [Fact]
    public void Recover_Brings_The_Child_Back_Once_The_Cause_Is_Gone()
    {
        var cut = Render<Ex066_ErrorBoundaryBasics>(p => p.Add(c => c.Explode, true));
        Assert.Equal(ExplodingChild.Message, cut.Find("#error").TextContent);

        cut.Render(p => p.Add(c => c.Explode, false));
        cut.Find("#recover").Click();

        cut.WaitForAssertion(() => Assert.Equal("ok", cut.Find("#child").TextContent));
    }

    // Non-vacuity for Recover(): re-rendering the parent alone does not clear a
    // boundary - it stays latched on its exception. So this proves the second error
    // was caught by a boundary that had genuinely been reset, not by a fresh one.
    [Fact]
    public void A_Recovered_Boundary_Catches_The_Next_Error_Too()
    {
        var cut = Render<Ex066_ErrorBoundaryBasics>(p => p.Add(c => c.Explode, true));
        cut.Render(p => p.Add(c => c.Explode, false));
        cut.Find("#recover").Click();
        cut.WaitForAssertion(() => Assert.Equal("ok", cut.Find("#child").TextContent));

        cut.Render(p => p.Add(c => c.Explode, true));

        Assert.Equal(ExplodingChild.Message, cut.Find("#error").TextContent);
    }
}
