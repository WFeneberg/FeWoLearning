using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex008_ScreenInitializeTests : CaliburnCoreContext
{
    private static Task Activate(Ex008_ScreenInitialize vm) => ((IActivate)vm).ActivateAsync();

    private static Task Deactivate(Ex008_ScreenInitialize vm, bool close) => ((IDeactivate)vm).DeactivateAsync(close);

    [Fact]
    public async Task First_ActivateAsync_Runs_OnInitializedAsync_Once_And_Sets_IsInitialized()
    {
        var vm = new Ex008_ScreenInitialize();
        Assert.False(vm.IsInitialized);

        await Activate(vm);

        Assert.Equal(1, vm.LoadCount);
        Assert.True(vm.IsInitialized);
    }

    [Fact]
    public async Task Activated_Event_Reports_WasInitialized_True_On_The_First_Activation()
    {
        var vm = new Ex008_ScreenInitialize();
        ActivationEventArgs? captured = null;
        vm.Activated += (_, e) => { captured = e; return Task.CompletedTask; };

        await Activate(vm);

        Assert.NotNull(captured);
        Assert.True(captured!.WasInitialized);
    }

    [Fact]
    public async Task Reactivating_An_Already_Active_Screen_Does_Not_Run_OnInitializedAsync_Again()
    {
        var vm = new Ex008_ScreenInitialize();
        await Activate(vm);
        Assert.Equal(1, vm.LoadCount);

        // Already active - per Caliburn this whole call is a no-op.
        await Activate(vm);

        Assert.Equal(1, vm.LoadCount);
    }

    [Fact]
    public async Task Deactivating_And_Reactivating_Does_Not_Run_OnInitializedAsync_Again()
    {
        var vm = new Ex008_ScreenInitialize();
        await Activate(vm);
        await Deactivate(vm, close: false);

        await Activate(vm);

        // The load-once hook already ran on the very first activation - a deactivate/
        // reactivate cycle must not run it a second time.
        Assert.Equal(1, vm.LoadCount);
        Assert.True(vm.IsInitialized);
    }

    [Fact]
    public async Task Activated_Event_Reports_WasInitialized_False_After_A_Reactivation()
    {
        var vm = new Ex008_ScreenInitialize();
        await Activate(vm);
        await Deactivate(vm, close: false);

        ActivationEventArgs? captured = null;
        vm.Activated += (_, e) => { captured = e; return Task.CompletedTask; };
        await Activate(vm);

        Assert.NotNull(captured);
        Assert.False(captured!.WasInitialized);
    }
}
