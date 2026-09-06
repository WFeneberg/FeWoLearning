using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex051_ConductorActivationChainTests : CaliburnCoreContext
{
    [Fact]
    public async Task SetActiveItemAsync_On_An_Inactive_Conductor_Sets_ActiveItem_And_Parent_Without_Activating_The_Child()
    {
        var conductor = new Ex051_ConductorActivationChain();
        var a = new Ex051_Child();

        await conductor.SetActiveItemAsync(a);

        // The sharp point of this exercise: ActiveItem is set immediately, but the child is not
        // active yet, and its OnActivatedAsync never ran - a stub that activates the conductor
        // first (copying ex033's ShowAsync order) fails right here.
        Assert.False(conductor.IsActive);
        Assert.Same(a, conductor.ActiveItem);
        Assert.Same(conductor, a.Parent);
        Assert.False(a.IsActive);
        Assert.Equal(0, a.ActivateCount);
    }

    [Fact]
    public async Task ActivateSelfAsync_Then_Activates_The_Already_Set_Child()
    {
        var conductor = new Ex051_ConductorActivationChain();
        var a = new Ex051_Child();
        await conductor.SetActiveItemAsync(a);

        await conductor.ActivateSelfAsync();

        Assert.True(conductor.IsActive);
        Assert.True(a.IsActive);
        Assert.Equal(1, a.ActivateCount);
    }

    [Fact]
    public async Task DeactivateSelfAsync_False_Deactivates_The_Child_With_Close_False_Not_True()
    {
        var conductor = new Ex051_ConductorActivationChain();
        var a = new Ex051_Child();
        await conductor.SetActiveItemAsync(a);
        await conductor.ActivateSelfAsync();

        await conductor.DeactivateSelfAsync(false);

        Assert.False(conductor.IsActive);
        Assert.False(a.IsActive);
        Assert.Equal(1, a.DeactivateCount);
        // A stub that hard-codes close: true here (the more "obvious" guess) fails on this
        // specific assertion.
        Assert.False(a.LastDeactivateWasClose);
    }

    [Fact]
    public async Task DeactivateSelfAsync_True_Deactivates_The_Child_With_Close_True_Not_False()
    {
        var conductor = new Ex051_ConductorActivationChain();
        var a = new Ex051_Child();
        await conductor.SetActiveItemAsync(a);
        await conductor.ActivateSelfAsync();

        await conductor.DeactivateSelfAsync(true);

        // Paired with the test above: proves the close flag is genuinely forwarded, not
        // hard-coded to either value.
        Assert.Equal(1, a.DeactivateCount);
        Assert.True(a.LastDeactivateWasClose);
    }

    [Fact]
    public async Task SetActiveItemAsync_On_An_Already_Active_Conductor_Immediately_Activates_The_Child()
    {
        var conductor = new Ex051_ConductorActivationChain();
        await conductor.ActivateSelfAsync();
        var a = new Ex051_Child();

        await conductor.SetActiveItemAsync(a);

        // Reverse order from the first test: the conductor was active BEFORE the item was ever
        // set, so this one call both sets ActiveItem and activates it - no separate step needed.
        Assert.True(a.IsActive);
        Assert.Equal(1, a.ActivateCount);
    }

    [Fact]
    public async Task Deactivating_With_Close_False_Does_Not_Clear_ActiveItem_So_Reactivating_Reactivates_The_Same_Child()
    {
        var conductor = new Ex051_ConductorActivationChain();
        var a = new Ex051_Child();
        await conductor.SetActiveItemAsync(a);
        await conductor.ActivateSelfAsync();
        await conductor.DeactivateSelfAsync(false);

        // Nothing re-sets the active item here - only the conductor is reactivated.
        await conductor.ActivateSelfAsync();

        Assert.Same(a, conductor.ActiveItem);
        Assert.True(a.IsActive);
        // The same child activated a second time, not a fresh one - a stub that clears
        // ActiveItem on deactivate (or never reaches OnActivatedAsync a second time) fails here.
        Assert.Equal(2, a.ActivateCount);
    }
}
