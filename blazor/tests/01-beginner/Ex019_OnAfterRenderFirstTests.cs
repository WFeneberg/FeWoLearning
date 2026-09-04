using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex019_OnAfterRenderFirstTests : BunitContext
{
    [Fact]
    public void Initial_Render_Counts_As_Both_A_Render_And_The_First_Render()
    {
        var cut = Render<Ex019_OnAfterRenderFirst>(p => p.Add(c => c.Label, "one"));

        // OnAfterRender runs after the render pass completes, so a bare
        // assertion right here could read the pre-callback value - wrap it.
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, cut.Instance.FirstRenderCount);
            Assert.Equal(1, cut.Instance.AfterRenderCount);
        });
    }

    [Fact]
    public void A_Later_Render_Increments_AfterRenderCount_But_Not_FirstRenderCount()
    {
        var cut = Render<Ex019_OnAfterRenderFirst>(p => p.Add(c => c.Label, "one"));
        cut.WaitForAssertion(() => Assert.Equal(1, cut.Instance.AfterRenderCount));

        cut.Render(p => p.Add(c => c.Label, "changed"));

        // Rejects both a "count every render as first render" implementation
        // and one that ignores firstRender entirely.
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(1, cut.Instance.FirstRenderCount);
            Assert.Equal(2, cut.Instance.AfterRenderCount);
        });
    }
}
