using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex052_ConductorCloseGuardTests : CaliburnCoreContext
{
    [Fact]
    public async Task ActivateBothAsync_Activates_The_Conductor_And_Both_Children_Simultaneously()
    {
        var conductor = new Ex052_ConductorCloseGuard();
        var a = new Ex052_Child();
        var b = new Ex052_Child();

        await conductor.ActivateBothAsync(a, b);

        Assert.True(conductor.IsActive);
        // AllActive: both stay active at the same time - neither replaces the other.
        Assert.True(a.IsActive);
        Assert.True(b.IsActive);
    }

    [Fact]
    public async Task Ex052_Child_CanCloseAsync_Returns_True_By_Default_And_False_When_RefuseClose_Is_Set_Recording_Each_Call()
    {
        var willing = new Ex052_Child();
        Assert.True(await willing.CanCloseAsync());
        Assert.Equal(1, willing.CanCloseAsyncCallCount);

        var refusing = new Ex052_Child { RefuseClose = true };
        Assert.False(await refusing.CanCloseAsync());
        Assert.Equal(1, refusing.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task Both_Children_Willing_The_Conductors_CanCloseAsync_Returns_True_And_Asks_Both_Children_Exactly_Once()
    {
        var conductor = new Ex052_ConductorCloseGuard();
        var a = new Ex052_Child();
        var b = new Ex052_Child();
        await conductor.ActivateBothAsync(a, b);

        var canClose = await conductor.CanCloseAsync();

        Assert.True(canClose);
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task One_Refusing_Child_Makes_The_Conductors_CanCloseAsync_Return_False_Even_Though_The_Other_Is_Willing()
    {
        var conductor = new Ex052_ConductorCloseGuard();
        var a = new Ex052_Child { RefuseClose = true };
        var b = new Ex052_Child();
        await conductor.ActivateBothAsync(a, b);

        var canClose = await conductor.CanCloseAsync();

        // A stub that answers true whenever ANY child is willing (an "OR" instead of the correct
        // "AND" over every child) fails right here.
        Assert.False(canClose);
        // Both children are genuinely asked - not just the refuser, and not short-circuited.
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task Calling_CanCloseAsync_Twice_Increments_Each_Childs_Call_Count_Again_Not_Just_Once()
    {
        var conductor = new Ex052_ConductorCloseGuard();
        var a = new Ex052_Child();
        var b = new Ex052_Child();
        await conductor.ActivateBothAsync(a, b);

        await conductor.CanCloseAsync();
        await conductor.CanCloseAsync();

        // The guard is a pure query, re-evaluated fresh every time - a memoized "ask once" stub
        // would leave these at 1 instead of 2.
        Assert.Equal(2, a.CanCloseAsyncCallCount);
        Assert.Equal(2, b.CanCloseAsyncCallCount);
        // And still just a query: neither child was actually closed by any of this.
        Assert.True(a.IsActive);
        Assert.True(b.IsActive);
    }
}
