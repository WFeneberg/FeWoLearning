using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex047_WindowManagerDialogTests : CaliburnViewContext
{
    // ScheduleTryClose/BoundedDialogAsync (used below) live on CaliburnViewContext -
    // ShowDialogAsync is modal, so the close must be scheduled BEFORE the call that reaches
    // it (here, host.ShowAsync), and the wait bounded, exactly as ShowDialogAndCloseAsync does
    // for the exercises that call ShowDialogAsync directly rather than through their own code.
    // BoundedDialogAsync always takes the root model too (not just the dialog task): a CORRECT
    // ShowAsync that awaits something before ever reaching ShowDialogAsync (a log call, a
    // guard, plain Task.Yield) makes ScheduleTryClose's own capture attempt run before any
    // window exists - the timeout escape must be able to find the window itself, fresh, rather
    // than trust that earlier capture.

    /// <summary>Wraps a real WindowManager and counts ShowDialogAsync calls, so a test can prove
    /// the INJECTED instance is the one Ex047_DialogHost actually used - a stub that built its
    /// own `new WindowManager()` internally would still make the dialog appear and close
    /// correctly, but this spy would never see the call.</summary>
    class SpyWindowManager : IWindowManager
    {
        readonly IWindowManager _inner = new WindowManager();
        public int ShowDialogCalls { get; private set; }

        public Task<bool?> ShowDialogAsync(object rootModel, object? context = null, IDictionary<string, object>? settings = null)
        {
            ShowDialogCalls++;
            return _inner.ShowDialogAsync(rootModel, context, settings);
        }

        public Task ShowWindowAsync(object rootModel, object? context = null, IDictionary<string, object>? settings = null) =>
            _inner.ShowWindowAsync(rootModel, context, settings);

        public Task ShowPopupAsync(object rootModel, object? context = null, IDictionary<string, object>? settings = null) =>
            _inner.ShowPopupAsync(rootModel, context, settings);
    }

    [WpfFact]
    public async Task ShowAsync_Closed_With_True_Resolves_To_True()
    {
        var vm = new Screen();
        var host = new Ex047_DialogHost(new SpyWindowManager());

        ScheduleTryClose(vm, true);
        var result = await BoundedDialogAsync(host.ShowAsync(vm, InvisibleDialogSettings()), vm);

        Assert.True(result);
    }

    [WpfFact]
    public async Task ShowAsync_Closed_With_False_Resolves_To_False()
    {
        var vm = new Screen();
        var host = new Ex047_DialogHost(new SpyWindowManager());

        ScheduleTryClose(vm, false);
        var result = await BoundedDialogAsync(host.ShowAsync(vm, InvisibleDialogSettings()), vm);

        Assert.False(result);
    }

    [WpfFact]
    public async Task ShowAsync_Uses_The_Injected_WindowManager_Not_A_Fresh_One()
    {
        var vm = new Screen();
        var spy = new SpyWindowManager();
        var host = new Ex047_DialogHost(spy);

        ScheduleTryClose(vm, true);
        await BoundedDialogAsync(host.ShowAsync(vm, InvisibleDialogSettings()), vm);

        // A stub that built its own `new WindowManager()` inside ShowAsync would still make the
        // previous two tests pass (the dialog really would show and close) but never touch spy.
        Assert.Equal(1, spy.ShowDialogCalls);
    }

    [WpfFact]
    public async Task TimesShown_Counts_Every_Call_On_The_Same_Host()
    {
        var host = new Ex047_DialogHost(new SpyWindowManager());

        var vm1 = new Screen();
        ScheduleTryClose(vm1, true);
        await BoundedDialogAsync(host.ShowAsync(vm1, InvisibleDialogSettings()), vm1);

        var vm2 = new Screen();
        ScheduleTryClose(vm2, false);
        await BoundedDialogAsync(host.ShowAsync(vm2, InvisibleDialogSettings()), vm2);

        Assert.Equal(2, host.TimesShown);
    }

    [WpfFact]
    public async Task Two_Hosts_With_Separate_WindowManagers_Do_Not_Share_State()
    {
        var spy1 = new SpyWindowManager();
        var spy2 = new SpyWindowManager();
        var host1 = new Ex047_DialogHost(spy1);
        var host2 = new Ex047_DialogHost(spy2);

        var vm = new Screen();
        ScheduleTryClose(vm, true);
        await BoundedDialogAsync(host1.ShowAsync(vm, InvisibleDialogSettings()), vm);

        // Only host1 (and therefore only spy1) was ever used.
        Assert.Equal(1, spy1.ShowDialogCalls);
        Assert.Equal(0, spy2.ShowDialogCalls);
        Assert.Equal(1, host1.TimesShown);
        Assert.Equal(0, host2.TimesShown);
    }
}
