using System.Collections.ObjectModel;
using System.Windows.Data;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex051_CollectionSynchronizationTests : WpfTestContext
{
    // Runs action on a fresh background thread and bounds the wait - a mutant that never
    // registers synchronization makes the mutation throw NotSupportedException, and an
    // unhandled exception on a bare Thread would otherwise crash the whole test process, not
    // just fail this test, so every path through here is caught and reported back instead.
    private static (bool completed, Exception? error) RunOnBackgroundThread(Action action)
    {
        Exception? caught = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                caught = ex;
            }
        })
        {
            IsBackground = true,
        };
        thread.Start();
        var completed = thread.Join(TimeSpan.FromSeconds(5));
        return (completed, caught);
    }

    [WpfFact]
    public void Mutator_Runs_Cleanly_Cross_Thread_Once_Synchronization_Is_Registered()
    {
        var items = new ObservableCollection<int> { 1, 2, 3 };
        // Arms the cross-thread check, the same way a real bound ItemsControl would - see
        // wpf/README.md: a bare ObservableCollection with no ICollectionView watching it never
        // throws regardless of thread, so a view must exist for this row to test anything.
        _ = CollectionViewSource.GetDefaultView(items);
        var gate = new object();

        var mutator = Ex051_CollectionSynchronization.PrepareSynchronizedMutator(items, gate, () => items.Add(4));
        var (completed, error) = RunOnBackgroundThread(mutator);

        // Against a mutant that never calls EnableCollectionSynchronization at all: the
        // background thread's mutation would throw NotSupportedException right here.
        Assert.True(completed, "background mutation did not finish within the bound");
        Assert.Null(error);
        Assert.Equal(4, items.Count);
        Assert.Contains(4, items);
    }

    [WpfFact]
    public void A_Different_Collection_And_Value_Also_Mutate_Cleanly()
    {
        // Varies the input across call sites, per wpf/README.md's own guidance.
        var items = new ObservableCollection<string> { "a", "b" };
        _ = CollectionViewSource.GetDefaultView(items);
        var gate = new object();

        var mutator = Ex051_CollectionSynchronization.PrepareSynchronizedMutator(items, gate, () => items.Add("z"));
        var (completed, error) = RunOnBackgroundThread(mutator);

        Assert.True(completed);
        Assert.Null(error);
        Assert.Equal(new[] { "a", "b", "z" }, items);
    }

    [WpfFact]
    public void Mutation_Holds_The_Exact_Gate_Object_Passed_To_EnableCollectionSynchronization()
    {
        // Load-bearing against a mutant that registers EnableCollectionSynchronization(items,
        // gate) correctly but locks on some OTHER object internally (or not at all) when
        // actually performing the mutation - registering the right gate is not the same as
        // USING it. Detected deterministically (no timing race): pause the background thread
        // WHILE it should be holding gate, then try to take gate ourselves with a zero
        // timeout - a correct implementation leaves that attempt failing (gate is genuinely
        // held elsewhere); a wrong one leaves gate free to grab.
        var items = new ObservableCollection<int> { 1 };
        _ = CollectionViewSource.GetDefaultView(items);
        var gate = new object();
        var enteredMutation = new ManualResetEventSlim(false);
        var releaseMutation = new ManualResetEventSlim(false);

        var mutator = Ex051_CollectionSynchronization.PrepareSynchronizedMutator(items, gate, () =>
        {
            enteredMutation.Set();
            releaseMutation.Wait(TimeSpan.FromSeconds(5));
            items.Add(99);
        });

        // Never let an exception escape unhandled on this bare Thread - a mutant that also
        // fails to register synchronization at all would otherwise crash the whole test
        // process here instead of just failing this one assertion (see the other tests'
        // RunOnBackgroundThread helper, which this test cannot reuse verbatim because it also
        // needs the thread reference itself to synchronize the TryEnter probe against).
        Exception? mutatorError = null;
        var thread = new Thread(() =>
        {
            try
            {
                mutator();
            }
            catch (Exception ex)
            {
                mutatorError = ex;
                enteredMutation.Set(); // unblock the wait below even if mutate() never ran
            }
        })
        { IsBackground = true };
        thread.Start();

        var signaled = enteredMutation.Wait(TimeSpan.FromSeconds(5));
        Assert.True(signaled, "mutation never started");

        var stoleGate = Monitor.TryEnter(gate, TimeSpan.Zero);
        if (stoleGate)
        {
            Monitor.Exit(gate);
        }

        releaseMutation.Set();
        var completed = thread.Join(TimeSpan.FromSeconds(5));

        Assert.True(completed);
        Assert.Null(mutatorError);
        Assert.False(stoleGate, "the mutation did not actually hold `gate` while it ran - a different lock object provides no real protection");
    }

    [WpfFact]
    public void Registration_Is_Already_Active_By_The_Time_PrepareSynchronizedMutator_Returns()
    {
        // Load-bearing against a mutant that registers synchronization lazily - only inside
        // the returned delegate, when (and if) it is actually invoked - instead of eagerly
        // before PrepareSynchronizedMutator itself returns. Probed WITHOUT ever invoking the
        // returned delegate at all: an entirely independent background thread, locked on the
        // SAME gate, mutates the collection directly. If synchronization was already active the
        // moment PrepareSynchronizedMutator returned, this unrelated mutation succeeds too;
        // if registration is deferred until the returned delegate itself runs, this one hits
        // an unregistered collection and throws.
        var items = new ObservableCollection<int> { 1, 2, 3 };
        _ = CollectionViewSource.GetDefaultView(items);
        var gate = new object();

        _ = Ex051_CollectionSynchronization.PrepareSynchronizedMutator(items, gate, () => { });

        var (completed, error) = RunOnBackgroundThread(() =>
        {
            lock (gate)
            {
                items.Add(99);
            }
        });

        Assert.True(completed);
        Assert.Null(error);
        Assert.Equal(4, items.Count);
    }
}
