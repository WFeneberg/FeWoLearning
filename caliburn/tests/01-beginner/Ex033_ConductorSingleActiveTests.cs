using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex033_ConductorSingleActiveTests : CaliburnCoreContext
{
    [Fact]
    public async Task CanCloseAsync_Returns_True_By_Default_And_False_When_RefuseClose_Is_Set()
    {
        var vm = new Ex033_Child();
        Assert.True(await vm.CanCloseAsync());

        vm.RefuseClose = true;
        Assert.False(await vm.CanCloseAsync());
    }

    [Fact]
    public async Task ShowAsync_Activates_The_Conductor_Itself_Before_Activating_The_Item()
    {
        var conductor = new Ex033_ConductorSingleActive();
        var a = new Ex033_Child();

        await conductor.ShowAsync(a);

        // ShowAsync alone had to activate the conductor - nothing else here does.
        Assert.True(conductor.IsActive);
        Assert.True(a.IsActive);
        Assert.Same(conductor, a.Parent);
    }

    [Fact]
    public async Task Showing_A_Second_Item_Deactivates_And_Closes_The_First()
    {
        var conductor = new Ex033_ConductorSingleActive();
        var a = new Ex033_Child();
        var b = new Ex033_Child();

        await conductor.ShowAsync(a);
        await conductor.ShowAsync(b);

        Assert.False(a.IsActive);
        Assert.Equal(1, a.DeactivateCount);
        // The defining behaviour of Conductor<T>: the item it replaces is CLOSED, not merely
        // deactivated. A stub that hard-codes false here (copying OneActive's behaviour)
        // fails right here.
        Assert.True(a.LastDeactivateWasClose);
        Assert.True(b.IsActive);
        Assert.Same(b, conductor.ActiveItem);
    }

    [Fact]
    public async Task A_Refusing_Item_Blocks_Replacement_And_Keeps_ActiveItem()
    {
        var conductor = new Ex033_ConductorSingleActive();
        var c = new Ex033_Child { RefuseClose = true };
        var d = new Ex033_Child();
        await conductor.ShowAsync(c);

        await conductor.ShowAsync(d);

        // A wrong implementation that ignores RefuseClose (always returns true) would let d
        // take over here - it does not.
        Assert.Same(c, conductor.ActiveItem);
        Assert.False(d.IsActive);
        Assert.Equal(0, c.DeactivateCount);
    }

    [Fact]
    public async Task Showing_A_Third_Item_Only_Closes_The_Item_That_Was_Actually_Active_At_The_Time()
    {
        var conductor = new Ex033_ConductorSingleActive();
        var a = new Ex033_Child();
        var b = new Ex033_Child();
        var c = new Ex033_Child();

        await conductor.ShowAsync(a);
        await conductor.ShowAsync(b);
        await conductor.ShowAsync(c);

        // a was closed exactly once, when b replaced it - showing c afterwards must not touch
        // a again.
        Assert.Equal(1, a.DeactivateCount);
        Assert.True(a.LastDeactivateWasClose);
        Assert.Equal(1, b.DeactivateCount);
        Assert.True(b.LastDeactivateWasClose);
        Assert.Equal(0, c.DeactivateCount);
        Assert.Same(c, conductor.ActiveItem);
    }

    [Fact]
    public async Task A_Conductor_That_Is_Never_Activated_Never_Truly_Activates_Its_Item()
    {
        // The ordering trap this whole batch is built around: a conductor only activates its
        // children while the conductor itself is active. Bypass ShowAsync deliberately and
        // call the framework's own ActivateItemAsync directly, without ever activating the
        // conductor, to prove the trap is real and not just something ShowAsync papers over.
        var conductor = new Ex033_ConductorSingleActive();
        var vm = new Ex033_Child();

        await conductor.ActivateItemAsync(vm);

        Assert.False(vm.IsActive);
        // Force this test red on the untouched stub via the guard - the assertion above never
        // reaches a throwing member on its own.
        Assert.True(await vm.CanCloseAsync());
    }
}
