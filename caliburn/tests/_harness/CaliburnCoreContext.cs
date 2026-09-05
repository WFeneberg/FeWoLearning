using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Base class for exercises with no view. Caliburn is configured through process-global
/// statics; a real app sets them once in a Bootstrapper, but a test run has to
/// re-establish them for EVERY test, because the previous test left its own behind.
/// </summary>
public abstract class CaliburnCoreContext
{
    // Captured exactly once, the first time any test in the run touches this base class --
    // before that test's own constructor body has mutated anything. ViewLocator.NameTransformer
    // starts with 4 built-in rules and is a process-global, writable static field: nothing in
    // Caliburn itself ever resets it, so ex015 (NameTransformerRule) leaks its AddRule into
    // every test that runs afterward unless something restores it.
    //
    // The explicit static constructor below is NOT decoration: a plain field initializer here
    // makes the compiler mark this type "beforefieldinit", which lets the JIT defer running it
    // lazily, at the first read of THIS class's own static field -- and that first read is the
    // "foreach (var rule in PristineNameTransformerRules)" a few lines into the instance
    // constructor, which runs AFTER that same constructor's "ViewLocator.NameTransformer.Clear()"
    // a line above it. Measured on this machine: with only a field initializer, the snapshot
    // came back Count=0 every time, because Clear() had already run before the lazy snapshot was
    // ever taken. An explicit static constructor removes beforefieldinit, which forces the CLR to
    // run this before ANY instance of the class can be constructed -- guaranteeing the snapshot
    // is taken while ViewLocator.NameTransformer still holds its pristine 4 rules.
    static readonly List<NameTransformer.Rule> PristineNameTransformerRules;

    static CaliburnCoreContext() => PristineNameTransformerRules = ViewLocator.NameTransformer.ToList();

    protected SimpleContainer Container { get; } = new();

    protected CaliburnCoreContext()
    {
        // Reset to the inline provider. A previous [WpfFact] may have installed the XAML
        // one, whose captured Dispatcher belongs to an STA thread that no longer pumps --
        // NotifyOfPropertyChange would then block until the call is cancelled, surfacing
        // as a TaskCanceledException from deep inside PropertyChangedBase.
        PlatformProvider.Current = new DefaultPlatformProvider();

        // The ViewLocator searches these assemblies. TrackMarker names whichever content
        // assembly this run is built against; the test assembly carries the harness's own
        // probe view.
        AssemblySource.Instance.Clear();
        AssemblySource.Instance.Add(typeof(TrackMarker).Assembly);
        AssemblySource.Instance.Add(typeof(CaliburnCoreContext).Assembly);

        // Not optional even with no UI at all: Coroutine.BeginExecute calls IoC.BuildUp,
        // so an otherwise pure-core coroutine test throws "IoC is not initialized".
        IoC.GetInstance = (service, key) =>
            Container.GetInstance(service, key) ?? Activator.CreateInstance(service)!;
        IoC.GetAllInstances = service => Container.GetAllInstances(service, null);
        IoC.BuildUp = Container.BuildUp;

        // Undo whatever a previous test's ViewLocator.NameTransformer.AddRule left behind.
        // Same "reset at the start of every test" design as the three globals above, not a
        // teardown -- there is no NameTransformer.RemoveRule, so restoring means clearing the
        // whole collection and re-adding the pristine snapshot, verified to round-trip
        // 4 -> AddRule -> 5 -> Clear+re-add -> 4.
        ViewLocator.NameTransformer.Clear();
        foreach (var rule in PristineNameTransformerRules)
            ViewLocator.NameTransformer.Add(rule);
    }
}
