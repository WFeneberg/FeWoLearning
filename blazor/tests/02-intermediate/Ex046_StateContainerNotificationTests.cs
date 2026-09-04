using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex046_StateContainerNotificationTests : BunitContext
{
    [Fact]
    public void Subscribes_Exactly_Once_After_Render()
    {
        Services.AddScoped<CounterStore>();

        Render<Ex046_StateContainerNotification>();
        var store = Services.GetRequiredService<CounterStore>();

        Assert.Equal(1, store.SubscriberCount);
    }

    // Non-vacuity: a component that never subscribes keeps rendering whatever it
    // read at construction time, so #value would stay "0" instead of advancing to "1".
    [Fact]
    public async Task A_Store_Change_Updates_The_Markup()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex046_StateContainerNotification>();
        var store = Services.GetRequiredService<CounterStore>();

        // The component's handler calls StateHasChanged() from this callback, so
        // it must run on the renderer's dispatcher, not this thread directly.
        await cut.InvokeAsync(() => store.Increment());

        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#value").TextContent));
    }

    // Non-vacuity: subscribing in OnParametersSet instead of OnInitialized
    // re-subscribes on every parameter push, so this cut.Render(...) would raise
    // the count to 2 instead of leaving it at 1. Kept bare (no WaitForAssertion) -
    // this is a "stayed the same" assertion, and cut.Render is synchronous anyway.
    [Fact]
    public void A_Parameter_Push_Does_Not_Add_A_Second_Subscription()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex046_StateContainerNotification>();
        var store = Services.GetRequiredService<CounterStore>();

        cut.Render(p => p.Add(c => c.Label, "Total"));

        Assert.Equal(1, store.SubscriberCount);
    }

    // Non-vacuity: a Dispose() that does not unsubscribe (or unsubscribes a
    // different delegate instance than the one it subscribed) leaves the store's
    // invocation list non-empty even once the component has been torn down.
    [Fact]
    public async Task Disposing_Unsubscribes()
    {
        Services.AddScoped<CounterStore>();

        Render<Ex046_StateContainerNotification>();
        var store = Services.GetRequiredService<CounterStore>();

        await DisposeComponentsAsync();

        Assert.Equal(0, store.SubscriberCount);
    }
}
