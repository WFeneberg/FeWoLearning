using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex043_ScopedStateContainerTests : BunitContext
{
    [Fact]
    public void Both_Readers_Start_At_Zero()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex043_ScopedStateContainer>();

        // Assert.All passes vacuously on an empty collection, so the count is checked
        // first; scoped to #both (the stub's own contractual wrapper) rather than
        // ".reading" everywhere, in case some other part of the page ever adds its own.
        var spans = cut.FindAll("#both .reading");
        Assert.Equal(2, spans.Count);
        Assert.All(spans, span => Assert.Equal("0", span.TextContent));
    }

    // Non-vacuity: a reader that never subscribes to Store.Changed keeps rendering
    // whatever it read at construction time, so a click on #bump would leave both
    // ".reading" spans stuck at "0" instead of advancing to "1".
    [Fact]
    public void Bumping_Updates_Both_Readers()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex043_ScopedStateContainer>();
        cut.Find("#bump").Click();

        cut.WaitForAssertion(() =>
        {
            var spans = cut.FindAll("#both .reading");
            Assert.Equal(2, spans.Count);
            Assert.All(spans, span => Assert.Equal("1", span.TextContent));
        });
    }

    [Fact]
    public void Both_Readers_Are_Subscribed_After_Render()
    {
        Services.AddScoped<CounterStore>();

        var cut = Render<Ex043_ScopedStateContainer>();
        var store = Services.GetRequiredService<CounterStore>();

        Assert.Equal(2, store.SubscriberCount);
    }

    // Non-vacuity: a reader whose Dispose() does not unsubscribe (or unsubscribes a
    // different delegate instance than the one it subscribed) leaves the store's
    // invocation list non-empty even once every component has been torn down.
    [Fact]
    public async Task Disposing_Unsubscribes_Both_Readers()
    {
        Services.AddScoped<CounterStore>();

        Render<Ex043_ScopedStateContainer>();
        var store = Services.GetRequiredService<CounterStore>();

        await DisposeComponentsAsync();

        Assert.Equal(0, store.SubscriberCount);
    }
}
