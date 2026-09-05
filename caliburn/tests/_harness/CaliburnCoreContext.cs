using System.Reflection;
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
    //
    // Two more forward risks this reset does NOT cover. First, the snapshot is SHALLOW: restoring
    // means re-adding the same four Rule instances, not four fresh copies, and Rule.ReplacePattern/
    // ReplacementValues are public, mutable fields - a test that mutates a built-in rule in place,
    // rather than adding a new one, is not undone by Clear()+re-add. Second, "pristine" only holds
    // because nothing touches ViewLocator.NameTransformer before this static constructor runs; a
    // future test class that reads or mutates it from its OWN static field initializer, before any
    // CaliburnCoreContext is ever constructed, would silently poison this snapshot for the whole run.
    static readonly List<NameTransformer.Rule> PristineNameTransformerRules;

    // Added for ex032 (BootstrapperConfigure): BootstrapperBase.Initialize()'s runtime path calls
    // AssemblySourceCache.Install() (see Caliburn.Micro's BootstrapperBase.StartRuntime()), which
    // permanently replaces AssemblySource.FindTypeByNames with a CACHED lookup that only ever
    // finds types satisfying AssemblySourceCache.ExtractTypes - by default, types assignable to
    // INotifyPropertyChanged (WPF's BootstrapperBase widens that to also include UIElement, which
    // is why ex013's ViewLocator - resolving TO a view, itself a UIElement - keeps working, while
    // ex016's ViewModelLocator - resolving TO a plain, non-notifying view model - stops finding
    // anything at all). AssemblySourceCache guards the swap with a private, never-reset
    // "isInstalled" flag, so this is a ONE-TIME, PERMANENT, PROCESS-GLOBAL mutation: once any
    // single test anywhere in the run calls a real BootstrapperBase.Initialize() (ex032 is the
    // first exercise that does), every OTHER test's plain-POCO view-model lookups would silently
    // start failing for the rest of the process - unless undone here, the same way the
    // NameTransformer leak above is undone.
    static readonly Func<IEnumerable<string>, Type> PristineFindTypeByNames;

    // Also added for ex032: StartRuntime() (see above) wraps AssemblySourceCache.ExtractTypes
    // in a NEW closure on every call, unconditionally - unlike FindTypeByNames, this wrapping is
    // NOT guarded by AssemblySourceCache's "isInstalled" flag, so it happens again every time
    // ANY BootstrapperBase.Initialize() runs on a fresh instance (a per-INSTANCE "isInitialized"
    // field, not a per-process one - a new bootstrapper always calls StartRuntime() again).
    // Install()'s CollectionChanged subscription on AssemblySource.Instance, once attached, is
    // itself permanent for the rest of the process, and fires on every Clear()/Add() below - so
    // without this reset, ex032's handful of Initialize() calls would leave an ever-deeper chain
    // of ExtractTypes wrappers that every one of THIS constructor's three AssemblySource.Instance
    // mutations, across all ~200 other tests, would re-invoke for no reason. Restoring
    // FindTypeByNames above already makes this chain inert for correctness (FindTypeByNames no
    // longer consults the cache ExtractTypes feeds at all), but resetting ExtractTypes too is
    // what stops the chain from growing forever - verified safe: ExtractTypes is read fresh by
    // the CollectionChanged handler on every invocation, never captured once by closure.
    static readonly Func<Assembly, IEnumerable<Type>> PristineExtractTypes;

    static CaliburnCoreContext()
    {
        PristineNameTransformerRules = ViewLocator.NameTransformer.ToList();
        PristineFindTypeByNames = AssemblySource.FindTypeByNames;
        PristineExtractTypes = AssemblySourceCache.ExtractTypes;
    }

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

        // Undo whatever ex032's (or any future exercise's) real BootstrapperBase.Initialize()
        // call left behind: AssemblySourceCache.Install() only ever runs its replacement in
        // once per process (see the static constructor's comment above), so restoring the
        // pristine, uncached delegate here is what makes that one-time install harmless.
        AssemblySource.FindTypeByNames = PristineFindTypeByNames;
        // ...and undo the ExtractTypes wrapping every Initialize() call adds - see the static
        // constructor's comment on PristineExtractTypes for why this one is NOT one-time.
        AssemblySourceCache.ExtractTypes = PristineExtractTypes;

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
