using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Tests.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex059_PersistComponentStateBasicsTests : BunitContext
{
    private const string Key = Ex059_PersistComponentStateBasics.StateKey;

    private readonly RecordingStateStore _store = new();

    /// Simulates the earlier (prerender) pass having persisted a count into the store,
    /// through the real serializer rather than a hand-written JSON literal.
    private async Task SeedPersistedCountAsync(int count)
    {
        var previous = PersistentStateHarness.CreateManager();
        await previous.RestoreStateAsync(new RecordingStateStore());
        previous.State.RegisterOnPersisting(() =>
        {
            previous.State.PersistAsJson(Key, count);
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

    /// What a later pass would restore out of the store.
    private async Task<int?> ReadBackAsync()
    {
        var next = PersistentStateHarness.CreateManager();
        await next.RestoreStateAsync(_store);
        return next.State.TryTakeFromJson<int>(Key, out var value) ? value : null;
    }

    [Fact]
    public async Task Starts_At_Zero_When_Nothing_Was_Persisted()
    {
        var manager = BeginPass();
        await manager.RestoreStateAsync(_store);

        var cut = Render<Ex059_PersistComponentStateBasics>();

        Assert.Equal("0", cut.Find("#count").TextContent);
    }

    [Fact]
    public async Task Adopts_The_Persisted_Count()
    {
        var manager = BeginPass();
        await SeedPersistedCountAsync(7);
        await manager.RestoreStateAsync(_store);

        var cut = Render<Ex059_PersistComponentStateBasics>();

        Assert.Equal("7", cut.Find("#count").TextContent);
    }

    // Non-vacuity: a callback that captures Count at registration time (or persists a
    // literal) writes 7 here, not 9. The clicks happen long after registration.
    [Fact]
    public async Task Persists_The_Count_As_It_Stands_When_The_Callback_Runs()
    {
        var manager = BeginPass();
        await SeedPersistedCountAsync(7);
        await manager.RestoreStateAsync(_store);
        var cut = Render<Ex059_PersistComponentStateBasics>();

        cut.Find("#inc").Click();
        cut.Find("#inc").Click();
        cut.WaitForAssertion(() => Assert.Equal("9", cut.Find("#count").TextContent));

        await manager.PersistStateAsync(_store, Renderer);

        Assert.Equal(9, await ReadBackAsync());
    }

    // Non-vacuity: registering only inside the "something was restored" branch is a
    // plausible misreading that passes every fact above and loses the first visit's
    // state entirely.
    [Fact]
    public async Task Registers_The_Callback_Even_When_Nothing_Was_Restored()
    {
        var manager = BeginPass();
        await manager.RestoreStateAsync(_store);
        var cut = Render<Ex059_PersistComponentStateBasics>();

        cut.Find("#inc").Click();
        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#count").TextContent));

        await manager.PersistStateAsync(_store, Renderer);

        Assert.Equal(1, await ReadBackAsync());
    }
}
