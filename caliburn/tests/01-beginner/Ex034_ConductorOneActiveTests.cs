using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex034_ConductorOneActiveTests : CaliburnCoreContext
{
    [Fact]
    public async Task CanCloseAsync_Returns_True_By_Default_And_False_When_RefuseClose_Is_Set()
    {
        var vm = new Ex034_ConductorOneActive();
        Assert.True(await vm.CanCloseAsync());

        vm.RefuseClose = true;
        Assert.False(await vm.CanCloseAsync());
    }

    [Fact]
    public async Task Activating_A_Second_Item_Keeps_Both_In_Items_But_Only_Deactivates_The_First_Without_Closing_It()
    {
        var conductor = new Conductor<Ex034_ConductorOneActive>.Collection.OneActive();
        await ((IActivate)conductor).ActivateAsync();
        var c1 = new Ex034_ConductorOneActive();
        var c2 = new Ex034_ConductorOneActive();

        await conductor.ActivateItemAsync(c1);
        Assert.IsType<BindableCollection<Ex034_ConductorOneActive>>(conductor.Items);

        await conductor.ActivateItemAsync(c2);

        Assert.Equal(2, conductor.Items.Count);
        Assert.Contains(c1, conductor.Items);
        Assert.Contains(c2, conductor.Items);
        Assert.False(c1.IsActive);
        Assert.Equal(1, c1.DeactivateCount);
        // The sharp contrast with Conductor<T> (ex033): displaced here means deactivated, not
        // closed. A stub that hard-codes true (copying ex033) fails right here.
        Assert.False(c1.LastDeactivateWasClose);
        Assert.Same(c2, conductor.ActiveItem);
    }

    [Fact]
    public async Task Closing_The_Active_Item_Removes_It_And_Promotes_The_Remaining_Item()
    {
        var conductor = new Conductor<Ex034_ConductorOneActive>.Collection.OneActive();
        await ((IActivate)conductor).ActivateAsync();
        var c1 = new Ex034_ConductorOneActive();
        var c2 = new Ex034_ConductorOneActive();
        await conductor.ActivateItemAsync(c1);
        await conductor.ActivateItemAsync(c2);

        await conductor.DeactivateItemAsync(c2, true);

        Assert.DoesNotContain(c2, conductor.Items);
        Assert.Single(conductor.Items);
        Assert.Equal(1, c2.DeactivateCount);
        Assert.True(c2.LastDeactivateWasClose);
        Assert.Same(c1, conductor.ActiveItem);
        Assert.True(c1.IsActive);
    }

    [Fact]
    public async Task A_Refusing_Item_Cannot_Be_Closed_And_Stays_Active_And_In_Items()
    {
        var conductor = new Conductor<Ex034_ConductorOneActive>.Collection.OneActive();
        await ((IActivate)conductor).ActivateAsync();
        var only = new Ex034_ConductorOneActive { RefuseClose = true };
        await conductor.ActivateItemAsync(only);

        await conductor.DeactivateItemAsync(only, true);

        // A wrong implementation that ignores RefuseClose would let this succeed - it does not.
        Assert.Contains(only, conductor.Items);
        Assert.True(only.IsActive);
        Assert.Equal(0, only.DeactivateCount);
    }

    [Fact]
    public async Task Activating_A_Third_Item_Only_Deactivates_The_Second_Items_Keeps_Growing()
    {
        var conductor = new Conductor<Ex034_ConductorOneActive>.Collection.OneActive();
        await ((IActivate)conductor).ActivateAsync();
        var g1 = new Ex034_ConductorOneActive();
        var g2 = new Ex034_ConductorOneActive();
        var g3 = new Ex034_ConductorOneActive();

        await conductor.ActivateItemAsync(g1);
        await conductor.ActivateItemAsync(g2);
        await conductor.ActivateItemAsync(g3);

        Assert.Equal(3, conductor.Items.Count);
        Assert.Equal(1, g1.DeactivateCount);
        Assert.False(g1.LastDeactivateWasClose);
        Assert.Equal(1, g2.DeactivateCount);
        Assert.False(g2.LastDeactivateWasClose);
        Assert.Equal(0, g3.DeactivateCount);
        Assert.Same(g3, conductor.ActiveItem);
    }
}
