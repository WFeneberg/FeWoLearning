using System.Windows.Input;
using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex020_RequerySuggestedTests : WpfTestContext
{
    [WpfFact]
    public void Invalidating_The_Command_Manager_Notifies_The_Observer()
    {
        var command = new Ex005_RelayCommand(_ => { });
        using var observer = new Ex020_RequeryObserver(command);
        var before = observer.Count;

        CommandManager.InvalidateRequerySuggested();
        Pump();

        // A per-instance delta, not a global count: CommandManager is process-global
        // and ex005's own tests leave a handler subscribed on it for the rest of the
        // run, so this only ever asserts against this observer's own starting point.
        Assert.True(observer.Count > before, "CanExecuteChanged must reach the observer after InvalidateRequerySuggested.");
    }

    [WpfFact]
    public void A_Second_Invalidate_While_One_Is_Still_Pending_Is_Coalesced_Into_One_Notification()
    {
        var command = new Ex005_RelayCommand(_ => { });
        using var observer = new Ex020_RequeryObserver(command);
        var before = observer.Count;

        // No Pump() between these two: InvalidateRequerySuggested posts at
        // DispatcherPriority.Background, so the first call is still pending when the
        // second arrives, and WPF swallows the second rather than queuing a duplicate.
        CommandManager.InvalidateRequerySuggested();
        CommandManager.InvalidateRequerySuggested();
        Pump();

        Assert.Equal(before + 1, observer.Count);
    }

    [WpfFact]
    public void Two_Invalidates_Separated_By_A_Pump_Each_Produce_Their_Own_Notification()
    {
        var command = new Ex005_RelayCommand(_ => { });
        using var observer = new Ex020_RequeryObserver(command);
        var before = observer.Count;

        CommandManager.InvalidateRequerySuggested();
        Pump();
        var afterFirst = observer.Count;
        Assert.True(afterFirst > before, "the first invalidate must already have produced a notification");

        // This is the contrast that proves the previous test is really about
        // coalescing a *pending* invalidation, not "only ever fires once, period".
        CommandManager.InvalidateRequerySuggested();
        Pump();

        Assert.True(observer.Count > afterFirst, "a second invalidate, issued once the first has actually been delivered, must produce its own notification too");
    }

    [WpfFact]
    public void Disposing_Stops_Further_Notifications()
    {
        var command = new Ex005_RelayCommand(_ => { });
        var observer = new Ex020_RequeryObserver(command);

        // Prove the subscription works before proving the removal works - asserting
        // only "nothing was raised after Dispose" would be satisfied by an
        // implementation where the subscription never worked either.
        CommandManager.InvalidateRequerySuggested();
        Pump();
        var whileSubscribed = observer.Count;
        Assert.True(whileSubscribed > 0, "the subscription must work before Dispose can be tested");

        observer.Dispose();
        CommandManager.InvalidateRequerySuggested();
        Pump();

        Assert.Equal(whileSubscribed, observer.Count);
    }

    // No forced-GC test here. An earlier draft tried to prove "weak handler storage"
    // by forcing a collection and asserting the observer kept receiving notifications
    // afterward - that direction is safe in theory (a still-rooted observer's own
    // fields can never be collected out from under it), but it was measured against a
    // deliberately broken implementation (an inline lambda subscribed with no field
    // anywhere) and the forced collection did not actually reclaim the orphaned
    // delegate in that run: the assertion passed on both the correct and the broken
    // code, so it was not load-bearing and was removed. Disposing_Stops_Further_
    // Notifications above is what actually forces the point: without somewhere to
    // store the handler, there is nothing to unsubscribe in Dispose, and that failure
    // is deterministic rather than dependent on GC timing.
}
