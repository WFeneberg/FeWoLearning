using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex052_ConductorCloseGuardTests : CaliburnCoreContext
{
    static async Task<Ex052_ConductorCloseGuard> ActiveConductorAsync(params Ex052_Child[] children)
    {
        var conductor = new Ex052_ConductorCloseGuard();
        await ((IActivate)conductor).ActivateAsync();
        foreach (var child in children)
            await conductor.ActivateItemAsync(child);
        return conductor;
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
        var a = new Ex052_Child();
        var b = new Ex052_Child();
        var conductor = await ActiveConductorAsync(a, b);

        var canClose = await conductor.CanCloseAsync();

        Assert.True(canClose);
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task One_Refusing_Child_First_Makes_The_Conductors_CanCloseAsync_Return_False_But_With_The_Default_Strategy_Touches_Neither_Child()
    {
        // The refuser is FIRST: a stub that short-circuits after the first refusal (instead of
        // the framework's real behaviour of asking everyone) would leave b's call count at 0.
        var a = new Ex052_Child { RefuseClose = true };
        var b = new Ex052_Child();
        var conductor = await ActiveConductorAsync(a, b);

        var canClose = await conductor.CanCloseAsync();

        Assert.False(canClose);
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
        // The scoped half of this exercise's own claim: with the DEFAULT close strategy (the one
        // this whole file uses), one refusal makes Children come back empty - so asking closes
        // NOTHING. This is not a property of CanCloseAsync in general (see ex053/ex054, where a
        // strategy that returns a willing subset makes this very call deactivate and remove it).
        Assert.True(a.IsActive);
        Assert.True(b.IsActive);
        Assert.Equal(2, conductor.Items.Count);
    }

    [Fact]
    public async Task Calling_CanCloseAsync_Twice_Increments_Each_Childs_Call_Count_Again_Not_Just_Once()
    {
        var a = new Ex052_Child();
        var b = new Ex052_Child();
        var conductor = await ActiveConductorAsync(a, b);

        await conductor.CanCloseAsync();
        await conductor.CanCloseAsync();

        // The guard is re-evaluated fresh every time - a memoized "ask once" stub would leave
        // these at 1 instead of 2.
        Assert.Equal(2, a.CanCloseAsyncCallCount);
        Assert.Equal(2, b.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task AllChildrenWillingToCloseAsync_Returns_True_When_Every_Child_Agrees_And_Asks_Each_Exactly_Once()
    {
        var a = new Ex052_Child();
        var b = new Ex052_Child();
        var conductor = await ActiveConductorAsync(a, b);

        var result = await conductor.AllChildrenWillingToCloseAsync();

        Assert.True(result);
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
    }

    [Fact]
    public async Task AllChildrenWillingToCloseAsync_Returns_False_When_The_First_Child_Refuses_But_Still_Asks_The_Second()
    {
        // Same short-circuit trap as the framework-level test above, but now against the
        // learner's OWN fold: a naive `foreach` that `return false` the moment it sees a refusal
        // never reaches b at all, leaving b's call count at 0 instead of 1.
        var a = new Ex052_Child { RefuseClose = true };
        var b = new Ex052_Child();
        var conductor = await ActiveConductorAsync(a, b);

        var result = await conductor.AllChildrenWillingToCloseAsync();

        Assert.False(result);
        Assert.Equal(1, a.CanCloseAsyncCallCount);
        Assert.Equal(1, b.CanCloseAsyncCallCount);
    }
}
