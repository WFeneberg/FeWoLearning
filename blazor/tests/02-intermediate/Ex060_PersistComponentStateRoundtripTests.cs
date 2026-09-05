using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

using DraftState = Ex060_PersistComponentStateRoundtrip.DraftState;

public class Ex060_PersistComponentStateRoundtripTests : BunitContext
{
    private const string Key = Ex060_PersistComponentStateRoundtrip.StateKey;

    [Fact]
    public void Starts_At_The_Default_Draft_When_Nothing_Was_Persisted()
    {
        AddBunitPersistentComponentState();

        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        Assert.Equal("untitled", cut.Find("#title").TextContent);
        Assert.Equal("", cut.Find("#tags").TextContent);
    }

    // The list is what makes this a typed round-trip rather than a scalar one: a
    // per-property persist ("title", "tags") cannot restore it in one TryTake.
    [Fact]
    public void Adopts_The_Whole_Persisted_Record()
    {
        var state = AddBunitPersistentComponentState();
        state.Persist(Key, new DraftState("spec", ["draft", "q3"]));

        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        Assert.Equal("spec", cut.Find("#title").TextContent);
        Assert.Equal("draft,q3", cut.Find("#tags").TextContent);
    }

    [Fact]
    public void Persists_The_Draft_As_It_Stands_When_The_Callback_Runs()
    {
        var state = AddBunitPersistentComponentState();
        var cut = Render<Ex060_PersistComponentStateRoundtrip>();

        cut.Find("#rename").Click();
        cut.WaitForAssertion(() => Assert.Equal("spec review", cut.Find("#title").TextContent));

        state.TriggerOnPersisting();

        Assert.True(state.TryTake<DraftState>(Key, out var persisted));
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
        var state = AddBunitPersistentComponentState();
        var cut = Render<Ex060_PersistComponentStateRoundtrip>();
        cut.Find("#rename").Click();
        cut.WaitForAssertion(() => Assert.Equal("spec review", cut.Find("#title").TextContent));

        await DisposeComponentsAsync();
        state.TriggerOnPersisting();

        Assert.False(state.TryTake<DraftState>(Key, out _));
    }
}
