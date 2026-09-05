using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Tests.Support;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

using DraftState = Ex060_PersistComponentStateRoundtrip.DraftState;

public class Ex060_PersistComponentStateRoundtripTests : BunitContext
{
    private const string Key = Ex060_PersistComponentStateRoundtrip.StateKey;

    private readonly RecordingStateStore _store = new();

    /// Simulates the earlier pass having persisted a whole draft, through the real
    /// serializer - so this asserts a round-trip, not a guessed JSON shape.
    private async Task SeedPersistedDraftAsync(DraftState draft)
    {
        var previous = PersistentStateHarness.CreateManager();
        await previous.RestoreStateAsync(new RecordingStateStore());
        previous.State.RegisterOnPersisting(() =>
        {
            previous.State.PersistAsJson(Key, draft);
            return Task.CompletedTask;
        });

        await previous.PersistStateAsync(_store, Renderer);
    }

    /// Opens the pass the component under test runs in, handing its
    /// PersistentComponentState to the component through DI. Registration has to come
    /// before anything resolves a service out of BunitContext.Services - which the
    /// Renderer property does - so a test opens the pass first, seeds what the earlier
    /// pass left behind, and only then restores.
    private ComponentStatePersistenceManager BeginPass()
    {
        var manager = PersistentStateHarness.CreateManager();
        Services.AddSingleton(manager.State);
        return manager;
    }

    private async Task<DraftState?> ReadBackAsync()
    {
        var next = PersistentStateHarness.CreateManager();
        await next.RestoreStateAsync(_store);
        return next.State.TryTakeFromJson<DraftState>(Key, out var draft) ? draft : null;
    }

    [Fact]
    public async Task Starts_At_The_Default_Draft_When_Nothing_Was_Persisted()
    {
        var manager = BeginPass();
        await manager.RestoreStateAsync(_store);

        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        Assert.Equal("untitled", cut.Find("#title").TextContent);
        Assert.Equal("", cut.Find("#tags").TextContent);
    }

    // The list is what makes this a typed round-trip rather than a scalar one: a
    // per-property persist ("title", "tags") cannot restore it in one TryTake.
    [Fact]
    public async Task Adopts_The_Whole_Persisted_Record()
    {
        var manager = BeginPass();
        await SeedPersistedDraftAsync(new DraftState("spec", ["draft", "q3"]));
        await manager.RestoreStateAsync(_store);

        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        Assert.Equal("spec", cut.Find("#title").TextContent);
        Assert.Equal("draft,q3", cut.Find("#tags").TextContent);
    }

    [Fact]
    public async Task Persists_The_Draft_As_It_Stands_When_The_Callback_Runs()
    {
        var manager = BeginPass();
        await manager.RestoreStateAsync(_store);
        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        cut.Find("#rename").Click();
        cut.WaitForAssertion(() => Assert.Equal("spec review", cut.Find("#title").TextContent));

        await manager.PersistStateAsync(_store, Renderer);

        var persisted = await ReadBackAsync();
        Assert.NotNull(persisted);
        Assert.Equal("spec review", persisted.Title);
        Assert.Equal(["edited"], persisted.Tags);
    }

    // Ruling: the pair of this fact and the one above is the whole point. The
    // registration outlives the component, so a Dispose() that does not give the
    // subscription back still writes the draft here - verified directly by leaving
    // _subscription.Dispose() out, which turns this fact red and nothing else.
    [Fact]
    public async Task Disposed_Component_Stops_Persisting()
    {
        var manager = BeginPass();
        await manager.RestoreStateAsync(_store);
        var cut = Render<Ex060_PersistComponentStateRoundtrip>();
        cut.Find("#rename").Click();
        cut.WaitForAssertion(() => Assert.Equal("spec review", cut.Find("#title").TextContent));

        await DisposeComponentsAsync();
        await manager.PersistStateAsync(_store, Renderer);

        Assert.False(_store.Persisted.ContainsKey(Key));
    }
}
