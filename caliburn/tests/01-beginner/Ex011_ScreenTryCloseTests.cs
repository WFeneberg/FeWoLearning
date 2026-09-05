using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex011_ScreenTryCloseTests : CaliburnCoreContext
{
    [Fact]
    public async Task CanCloseAsync_Returns_True_By_Default()
    {
        var vm = new Ex011_ScreenTryClose();

        Assert.True(await vm.CanCloseAsync());
    }

    [Fact]
    public async Task CanCloseAsync_Returns_False_When_RefuseClose_Is_Set()
    {
        var vm = new Ex011_ScreenTryClose { RefuseClose = true };

        Assert.False(await vm.CanCloseAsync());
    }

    [Fact]
    public async Task TryCloseAsync_On_A_Parentless_Viewless_Active_Screen_Is_A_Silent_NoOp()
    {
        var vm = new Ex011_ScreenTryClose();
        await ((IActivate)vm).ActivateAsync();

        // The guard itself works fine in isolation - this call also forces this test red on
        // the untouched stub, since the no-op path below never reaches CanCloseAsync at all.
        Assert.True(await vm.CanCloseAsync());

        await vm.TryCloseAsync();

        // Measured: with no Parent and no attached view, TryCloseAsync never even calls
        // OnDeactivateAsync - the screen just stays active. A learner expecting "TryClose
        // always deactivates" gets caught here.
        Assert.Equal(0, vm.DeactivateCount);
        Assert.True(vm.IsActive);
    }

    [Fact]
    public async Task TryCloseAsync_Under_An_Active_Conductor_Deactivates_With_Close_True_And_Leaves_Items()
    {
        var conductor = new Conductor<Ex011_ScreenTryClose>.Collection.OneActive();
        // The conductor itself must be active before it will activate a child at all.
        await ((IActivate)conductor).ActivateAsync();
        var vm = new Ex011_ScreenTryClose();
        await conductor.ActivateItemAsync(vm);
        Assert.True(vm.IsActive);

        await vm.TryCloseAsync();

        Assert.Equal(1, vm.DeactivateCount);
        Assert.True(vm.LastDeactivateWasClose);
        Assert.False(vm.IsActive);
        Assert.DoesNotContain(vm, conductor.Items);
    }

    [Fact]
    public async Task TryCloseAsync_Refused_By_The_Guard_Leaves_The_Screen_Active_And_In_Items()
    {
        var conductor = new Conductor<Ex011_ScreenTryClose>.Collection.OneActive();
        await ((IActivate)conductor).ActivateAsync();
        var vm = new Ex011_ScreenTryClose { RefuseClose = true };
        await conductor.ActivateItemAsync(vm);

        await vm.TryCloseAsync();

        // Refused - never deactivated, never removed.
        Assert.Equal(0, vm.DeactivateCount);
        Assert.True(vm.IsActive);
        Assert.Contains(vm, conductor.Items);
    }

    [Fact]
    public async Task A_Screen_Parented_By_An_Inactive_Conductor_Was_Never_Truly_Activated_And_TryCloseAsync_Ignores_It_Too()
    {
        // The ordering trap: a conductor only activates its children while the conductor
        // itself is active. Deliberately do NOT activate the conductor here - the child gets a
        // Parent (unlike the earlier parentless no-op test), but IsActive stays false, because
        // it was never truly activated.
        var conductor = new Conductor<Ex011_ScreenTryClose>.Collection.OneActive();
        var vm = new Ex011_ScreenTryClose();
        await conductor.ActivateItemAsync(vm);
        Assert.False(vm.IsActive);

        // Force this test red on the untouched stub via the guard.
        Assert.True(await vm.CanCloseAsync());

        await vm.TryCloseAsync();

        // Deactivating a never-activated screen is itself a no-op - even though it has a
        // Parent, unlike the earlier parentless test.
        Assert.Equal(0, vm.DeactivateCount);
        Assert.False(vm.IsActive);
    }
}
