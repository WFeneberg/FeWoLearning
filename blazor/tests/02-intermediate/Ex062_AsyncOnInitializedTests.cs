using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex062_AsyncOnInitializedTests : BunitContext
{
    // RunContinuationsAsynchronously keeps SetResult from running the component's
    // continuation inline on the test thread.
    private static TaskCompletionSource<IReadOnlyList<string>> Gate()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public void Renders_The_Loading_State_While_The_Load_Is_Pending()
    {
        var gate = Gate();

        var cut = Render<Ex062_AsyncOnInitialized>(p => p.Add(c => c.Load, () => gate.Task));

        Assert.Equal("loading…", cut.Find("#loading").TextContent);
        Assert.Empty(cut.FindAll("#items"));
    }

    [Fact]
    public void Renders_The_Items_Once_The_Load_Completes()
    {
        var gate = Gate();
        var cut = Render<Ex062_AsyncOnInitialized>(p => p.Add(c => c.Load, () => gate.Task));

        gate.SetResult(["alpha", "beta"]);

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll("#items li").Count));
        Assert.Equal("alpha", cut.FindAll("#items li")[0].TextContent);
        Assert.Empty(cut.FindAll("#loading"));
    }

    // Non-vacuity: proves the loading flag is actually cleared rather than the
    // component simply never having rendered a loading state.
    [Fact]
    public void An_Already_Completed_Load_Skips_The_Loading_State()
    {
        var cut = Render<Ex062_AsyncOnInitialized>(p => p.Add(
            c => c.Load,
            () => Task.FromResult<IReadOnlyList<string>>(["only"])));

        Assert.Empty(cut.FindAll("#loading"));
        Assert.Equal("only", cut.Find("#items li").TextContent);
    }

    // The awaited call must happen once per component, from the initialization pass -
    // not per render, which is what OnParametersSetAsync or OnAfterRenderAsync would
    // give instead.
    [Fact]
    public void Loads_Exactly_Once()
    {
        var calls = 0;
        Func<Task<IReadOnlyList<string>>> load = () =>
        {
            calls++;
            return Task.FromResult<IReadOnlyList<string>>(["only"]);
        };
        var cut = Render<Ex062_AsyncOnInitialized>(p => p.Add(c => c.Load, load));

        cut.Render(p => p.Add(c => c.Load, load));
        cut.Render(p => p.Add(c => c.Load, load));

        Assert.Equal(1, calls);
    }
}
