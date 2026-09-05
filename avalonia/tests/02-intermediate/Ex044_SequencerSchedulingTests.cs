using System.Windows.Input;
using ReactiveUI.Primitives.Concurrency;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex044_SequencerSchedulingTests
{
    [Fact]
    public void Starts_Hidden()
    {
        var vm = new Ex044_SequencerSchedulingViewModel(new VirtualClock());

        Assert.False(vm.IsVisible);
    }

    [Fact]
    public void Showing_Makes_It_Visible_Immediately()
    {
        var vm = new Ex044_SequencerSchedulingViewModel(new VirtualClock());

        ((ICommand)vm.ShowCommand).Execute(null);

        Assert.True(vm.IsVisible);
    }

    // The discriminator: a solution that ignores the injected scheduler (using
    // Sequencer.Default or Sequencer.CurrentThread hard-coded instead) will not
    // respond to AdvanceBy at all - IsVisible would either never go false within
    // this test, or go false immediately regardless of elapsed virtual time. Only a
    // solution that schedules through the INJECTED VirtualClock hides at exactly
    // the 2-second mark and not a moment before.
    [Fact]
    public void Hides_Itself_At_Exactly_Two_Seconds_Of_Virtual_Time_Not_Before()
    {
        var vt = new VirtualClock();
        var vm = new Ex044_SequencerSchedulingViewModel(vt);

        ((ICommand)vm.ShowCommand).Execute(null);
        Assert.True(vm.IsVisible);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(1999));
        Assert.True(vm.IsVisible);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(1));
        Assert.False(vm.IsVisible);
    }

    // A second, independent Show/hide cycle on the same view model and clock: guards
    // against a one-shot wiring that only ever schedules once.
    [Fact]
    public void Showing_A_Second_Time_Schedules_A_Fresh_Two_Second_Hide()
    {
        var vt = new VirtualClock();
        var vm = new Ex044_SequencerSchedulingViewModel(vt);

        ((ICommand)vm.ShowCommand).Execute(null);
        vt.AdvanceBy(TimeSpan.FromSeconds(2));
        Assert.False(vm.IsVisible);

        ((ICommand)vm.ShowCommand).Execute(null);
        Assert.True(vm.IsVisible);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(1999));
        Assert.True(vm.IsVisible);

        vt.AdvanceBy(TimeSpan.FromMilliseconds(1));
        Assert.False(vm.IsVisible);
    }
}
