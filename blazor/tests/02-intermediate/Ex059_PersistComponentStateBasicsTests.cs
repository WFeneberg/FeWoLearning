using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

// bUnit's own test double for PersistentComponentState: Persist() seeds what an
// earlier render pass left behind, TriggerOnPersisting() runs the callbacks the
// component registered, and TryTake() reads back what they wrote. It is not in the
// default services - AddBunitPersistentComponentState() puts it there, and like every
// service registration that has to happen before the first render.
public class Ex059_PersistComponentStateBasicsTests : BunitContext
{
    private const string Key = Ex059_PersistComponentStateBasics.StateKey;

    [Fact]
    public void Starts_At_Zero_When_Nothing_Was_Persisted()
    {
        AddBunitPersistentComponentState();

        var cut = Render<Ex059_PersistComponentStateBasics>();

        Assert.Equal("0", cut.Find("#count").TextContent);
    }

    [Fact]
    public void Adopts_The_Persisted_Count()
    {
        var state = AddBunitPersistentComponentState();
        state.Persist(Key, 7);

        var cut = Render<Ex059_PersistComponentStateBasics>();

        Assert.Equal("7", cut.Find("#count").TextContent);
    }

    // Non-vacuity: a callback that captures Count at registration time (or persists a
    // literal) writes 7 here, not 9. The clicks happen long after registration.
    [Fact]
    public void Persists_The_Count_As_It_Stands_When_The_Callback_Runs()
    {
        var state = AddBunitPersistentComponentState();
        state.Persist(Key, 7);
        var cut = Render<Ex059_PersistComponentStateBasics>();

        cut.Find("#inc").Click();
        cut.Find("#inc").Click();
        cut.WaitForAssertion(() => Assert.Equal("9", cut.Find("#count").TextContent));

        state.TriggerOnPersisting();

        Assert.True(state.TryTake<int>(Key, out var persisted));
        Assert.Equal(9, persisted);
    }

    // Non-vacuity: registering only inside the "something was restored" branch is a
    // plausible misreading that passes every fact above and loses the first visit's
    // state entirely.
    [Fact]
    public void Registers_The_Callback_Even_When_Nothing_Was_Restored()
    {
        var state = AddBunitPersistentComponentState();
        var cut = Render<Ex059_PersistComponentStateBasics>();

        cut.Find("#inc").Click();
        cut.WaitForAssertion(() => Assert.Equal("1", cut.Find("#count").TextContent));

        state.TriggerOnPersisting();

        Assert.True(state.TryTake<int>(Key, out var persisted));
        Assert.Equal(1, persisted);
    }
}
