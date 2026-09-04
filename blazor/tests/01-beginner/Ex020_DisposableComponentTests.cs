using Bunit;
using FeWoLearning.Blazor.Exercises.Beginner;
using FeWoLearning.Blazor.Support;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Beginner;

public class Ex020_DisposableComponentTests : BunitContext
{
    [Fact]
    public void Rendering_Subscribes_To_The_Ticker()
    {
        var ticker = new Ticker();

        Render<Ex020_DisposableComponent>(p => p.Add(c => c.Ticker, ticker));

        Assert.Equal(1, ticker.SubscriberCount);
    }

    [Fact]
    public async Task Ticking_The_Ticker_Increments_The_Displayed_Count()
    {
        var ticker = new Ticker();
        var cut = Render<Ex020_DisposableComponent>(p => p.Add(c => c.Ticker, ticker));

        // The component's handler calls StateHasChanged() from the ticker's
        // callback, so it must run on the renderer's dispatcher, not this
        // thread directly - hence cut.InvokeAsync rather than a bare call.
        await cut.InvokeAsync(() => ticker.Tick());
        await cut.InvokeAsync(() => ticker.Tick());

        // The increment happens on the ticker's callback, not in a render
        // pass triggered by this test - wait for it to land.
        cut.WaitForAssertion(() => Assert.Equal("2", cut.Find("#ticks").TextContent));
    }

    [Fact]
    public async Task Disposing_The_Component_Unsubscribes_From_The_Ticker()
    {
        var ticker = new Ticker();
        Render<Ex020_DisposableComponent>(p => p.Add(c => c.Ticker, ticker));

        await DisposeComponentsAsync();

        // The classic leak this exercise teaches: a component that subscribes
        // but never unsubscribes leaves the ticker's count non-zero forever.
        Assert.Equal(0, ticker.SubscriberCount);
    }
}
