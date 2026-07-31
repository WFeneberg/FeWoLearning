using System.Threading.Tasks;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex093_BlazorCounterComponentTests
{
    [Fact]
    public void InitialRender_ShowsZero()
    {
        var cut = new BlazorCounterComponent();

        Assert.Equal(0, cut.Count);
        Assert.Equal(1, cut.RenderCount);
        Assert.Equal(
            "<div class=\"counter\"><p role=\"status\">Current count: 0</p><button>Click me</button></div>",
            cut.Markup);
    }

    [Fact]
    public async Task SingleClick_IncrementsCountAndRerenders()
    {
        var cut = new BlazorCounterComponent();

        await cut.HandleClickAsync(); // simulate cut.Find("button").Click()

        Assert.Equal(1, cut.Count);
        Assert.Equal(2, cut.RenderCount);
        Assert.Contains("Current count: 1", cut.Markup);
    }

    [Fact]
    public async Task SequentialClicks_AccumulateAndRenderOncePerClick()
    {
        var cut = new BlazorCounterComponent();

        for (var i = 0; i < 5; i++)
            await cut.HandleClickAsync();

        Assert.Equal(5, cut.Count);
        Assert.Equal(6, cut.RenderCount); // 1 initial + 5 clicks
        Assert.Equal(
            "<div class=\"counter\"><p role=\"status\">Current count: 5</p><button>Click me</button></div>",
            cut.Markup);
    }

    [Fact]
    public async Task ConcurrentClicks_AreSerializedWithNoLostUpdates()
    {
        var cut = new BlazorCounterComponent();

        // Simulate three rapid clicks fired "at once" (e.g. a double-click
        // plus a stray extra event) — the renderer must still process each
        // one to completion without losing an increment or a render.
        var click1 = cut.HandleClickAsync();
        var click2 = cut.HandleClickAsync();
        var click3 = cut.HandleClickAsync();
        await Task.WhenAll(click1, click2, click3);

        Assert.Equal(3, cut.Count);
        Assert.Equal(4, cut.RenderCount); // 1 initial + 3 clicks, none lost
        Assert.Equal(
            "<div class=\"counter\"><p role=\"status\">Current count: 3</p><button>Click me</button></div>",
            cut.Markup);
    }
}
