using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

using ReportData = Ex087_ComponentStatePreservationAcrossRenderMode.ReportData;

public class Ex087_ComponentStatePreservationAcrossRenderModeTests : BunitContext
{
    private const string Key = Ex087_ComponentStatePreservationAcrossRenderMode.StateKey;

    private int _loads;

    private ReportData Load()
    {
        _loads++;
        return new ReportData("computed", 42);
    }

    [Fact]
    public void With_Nothing_Persisted_It_Does_The_Work()
    {
        AddBunitPersistentComponentState();

        var cut = Render<Ex087_ComponentStatePreservationAcrossRenderMode>(
            p => p.Add(c => c.Load, Load));

        Assert.Equal("computed", cut.Find("#report").TextContent);
        Assert.Equal("42", cut.Find("#rows").TextContent);
        Assert.Equal(1, _loads);
    }

    // Ruling: this is the row. A component that loads first and then overwrites the
    // result with the restored value renders exactly the same thing and still pays
    // for the query - so the assertion that matters is the call count, not the
    // markup.
    [Fact]
    public void With_State_Persisted_It_Adopts_It_And_Does_No_Work_At_All()
    {
        var state = AddBunitPersistentComponentState();
        state.Persist(Key, new ReportData("from the prerender", 7));

        var cut = Render<Ex087_ComponentStatePreservationAcrossRenderMode>(
            p => p.Add(c => c.Load, Load));

        Assert.Equal("from the prerender", cut.Find("#report").TextContent);
        Assert.Equal("7", cut.Find("#rows").TextContent);
        Assert.Equal(0, _loads);
    }

    [Fact]
    public void It_Persists_What_It_Ended_Up_With_For_The_Next_Pass()
    {
        var state = AddBunitPersistentComponentState();
        Render<Ex087_ComponentStatePreservationAcrossRenderMode>(p => p.Add(c => c.Load, Load));

        state.TriggerOnPersisting();

        Assert.True(state.TryTake<ReportData>(Key, out var persisted));
        Assert.NotNull(persisted);
        Assert.Equal("computed", persisted.Title);
        Assert.Equal(42, persisted.RowCount);
    }

    // Non-vacuity for registering in both branches: a component that only registers
    // the callback when it had to do the work leaves the chain broken after the
    // first handover.
    [Fact]
    public void It_Registers_The_Callback_Even_When_It_Restored()
    {
        var state = AddBunitPersistentComponentState();
        state.Persist(Key, new ReportData("from the prerender", 7));
        Render<Ex087_ComponentStatePreservationAcrossRenderMode>(p => p.Add(c => c.Load, Load));

        state.TriggerOnPersisting();

        Assert.True(state.TryTake<ReportData>(Key, out var persisted));
        Assert.Equal("from the prerender", persisted!.Title);
    }
}
