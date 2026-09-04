using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex009_ScreenActivateTests : CaliburnCoreContext
{
    private static Task Activate(Ex009_ScreenActivate vm) => ((IActivate)vm).ActivateAsync();

    private static Task Deactivate(Ex009_ScreenActivate vm, bool close) => ((IDeactivate)vm).DeactivateAsync(close);

    [Fact]
    public async Task First_ActivateAsync_Runs_OnActivatedAsync_Once_And_Sets_IsActive()
    {
        var vm = new Ex009_ScreenActivate();
        Assert.False(vm.IsActive);
        var activations = new List<ActivationEventArgs>();
        vm.Activated += (_, e) => { activations.Add(e); return Task.CompletedTask; };

        await Activate(vm);

        Assert.Equal(1, vm.ActivateCount);
        Assert.True(vm.IsActive);
        Assert.Single(activations);
    }

    [Fact]
    public async Task Reactivating_An_Already_Active_Screen_Does_Nothing_At_All()
    {
        var vm = new Ex009_ScreenActivate();
        await Activate(vm);
        Assert.Equal(1, vm.ActivateCount);

        var changes = new List<string?>();
        vm.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        var activations = new List<ActivationEventArgs>();
        vm.Activated += (_, e) => { activations.Add(e); return Task.CompletedTask; };

        await Activate(vm);

        // Not "runs again with the same result" - genuinely nothing happens: no hook call,
        // no PropertyChanged, no Activated event.
        Assert.Equal(1, vm.ActivateCount);
        Assert.Empty(changes);
        Assert.Empty(activations);
    }

    [Fact]
    public async Task DeactivateAsync_False_Runs_OnDeactivateAsync_And_Clears_IsActive()
    {
        var vm = new Ex009_ScreenActivate();
        await Activate(vm);

        await Deactivate(vm, close: false);

        Assert.Equal(1, vm.DeactivateCount);
        Assert.False(vm.LastDeactivateWasClose);
        Assert.False(vm.IsActive);
    }

    [Fact]
    public async Task DeactivateAsync_On_A_Never_Activated_Screen_Does_Nothing_At_All()
    {
        var vm = new Ex009_ScreenActivate();

        // Never activated - per Caliburn this call never reaches OnDeactivateAsync at all.
        await Deactivate(vm, close: false);
        Assert.Equal(0, vm.DeactivateCount);

        // Drives the exercise's own hook too, so this test still fails red on the untouched
        // stub instead of only ever proving framework behaviour the learner had no part in.
        await Activate(vm);

        Assert.Equal(1, vm.ActivateCount);
        Assert.True(vm.IsActive);
    }

    [Fact]
    public async Task Deactivating_With_Close_True_Reports_WasClosed_On_The_Deactivated_Event()
    {
        var vm = new Ex009_ScreenActivate();
        DeactivationEventArgs? captured = null;
        vm.Deactivated += (_, e) => { captured = e; return Task.CompletedTask; };
        await Activate(vm);

        await Deactivate(vm, close: true);

        Assert.NotNull(captured);
        Assert.True(captured!.WasClosed);
        Assert.Equal(1, vm.DeactivateCount);
        Assert.True(vm.LastDeactivateWasClose);
    }
}
