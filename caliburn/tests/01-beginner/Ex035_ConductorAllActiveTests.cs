using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex035_ConductorAllActiveTests : CaliburnCoreContext
{
    [Fact]
    public async Task CanCloseAsync_Returns_True_By_Default_And_False_When_RefuseClose_Is_Set()
    {
        var vm = new Ex035_ConductorAllActive();
        Assert.True(await vm.CanCloseAsync());

        vm.RefuseClose = true;
        Assert.False(await vm.CanCloseAsync());
    }

    [Fact]
    public async Task Activating_A_Second_Item_Leaves_Both_Simultaneously_Active()
    {
        var conductor = new Conductor<Ex035_ConductorAllActive>.Collection.AllActive();
        await ((IActivate)conductor).ActivateAsync();
        var h1 = new Ex035_ConductorAllActive();
        var h2 = new Ex035_ConductorAllActive();

        await conductor.ActivateItemAsync(h1);
        await conductor.ActivateItemAsync(h2);

        // The defining behaviour of AllActive: activating h2 never deactivates h1, unlike
        // Conductor<T> (ex033) or OneActive (ex034).
        Assert.True(h1.IsActive);
        Assert.True(h2.IsActive);
        Assert.Equal(2, conductor.Items.Count);
        // Force this test red on the untouched stub via the guard - nothing above calls a
        // throwing member on its own.
        Assert.True(await h1.CanCloseAsync());
    }

    [Fact]
    public async Task Closing_One_Item_Deactivates_Only_That_Item_The_Other_Stays_Active()
    {
        var conductor = new Conductor<Ex035_ConductorAllActive>.Collection.AllActive();
        await ((IActivate)conductor).ActivateAsync();
        var h1 = new Ex035_ConductorAllActive();
        var h2 = new Ex035_ConductorAllActive();
        await conductor.ActivateItemAsync(h1);
        await conductor.ActivateItemAsync(h2);

        await conductor.DeactivateItemAsync(h1, true);

        Assert.False(h1.IsActive);
        Assert.Equal(1, h1.DeactivateCount);
        Assert.True(h1.LastDeactivateWasClose);
        Assert.DoesNotContain(h1, conductor.Items);
        // h2 is completely unaffected by closing h1 - no cross-talk between items.
        Assert.True(h2.IsActive);
        Assert.Contains(h2, conductor.Items);
        Assert.Equal(0, h2.DeactivateCount);
    }

    [Fact]
    public async Task A_Refusing_Item_Cannot_Be_Closed_And_Stays_Active_And_In_Items()
    {
        var conductor = new Conductor<Ex035_ConductorAllActive>.Collection.AllActive();
        await ((IActivate)conductor).ActivateAsync();
        var only = new Ex035_ConductorAllActive { RefuseClose = true };
        await conductor.ActivateItemAsync(only);

        await conductor.DeactivateItemAsync(only, true);

        // A wrong implementation that ignores RefuseClose would let this succeed - it does not.
        Assert.True(only.IsActive);
        Assert.Contains(only, conductor.Items);
        Assert.Equal(0, only.DeactivateCount);
    }

    [Fact]
    public async Task AllActive_Has_No_ActiveItem_Property()
    {
        var vm = new Ex035_ConductorAllActive();
        // Force this test red on the untouched stub - the reflection check below never calls
        // a throwing member on its own.
        Assert.True(await vm.CanCloseAsync());

        // Measured: AllActive does not inherit ConductorBaseWithActiveItem<T> the way
        // Conductor<T> and OneActive both do - "the one active item" is not a concept it has.
        var property = typeof(Conductor<Ex035_ConductorAllActive>.Collection.AllActive).GetProperty("ActiveItem");
        Assert.Null(property);
    }
}
