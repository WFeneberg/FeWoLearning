// Exercise 064 - Design Time Detection (intermediate).
// Goal:   Execute.InDesignMode is NOT a fact about the process - it is whatever the currently
//         installed IPlatformProvider's InDesignMode answers. Learn this by writing a custom
//         IPlatformProvider that reports design mode however it is told to, independent of
//         whatever provider it wraps, and a view model that reads Execute.InDesignMode itself to
//         decide between canned design-time data and its real value.
// Drills: implementing IPlatformProvider.InDesignMode on a provider that wraps another provider
//         for everything else (Execute still needs somewhere real to marshal work through), and
//         writing a computed property that branches on Execute.InDesignMode rather than on some
//         constructor flag - so swapping which provider is installed changes the ANSWER without
//         the view model itself ever being touched.
// Passes: dotnet test --filter FullyQualifiedName~Ex064_
//
// Measured on this machine (Caliburn.Micro 5.0.258): XamlPlatformProvider (installed by this
// track's view harness) reports Execute.InDesignMode == FALSE. DefaultPlatformProvider
// (installed by the viewless harness) reports it TRUE - the surprising direction: a harness
// with no view at all measures as "design time". IPlatformProvider's full member list is
// InDesignMode, PropertyChangeNotificationsOnUIThread, OnUIThread, OnUIThreadAsync,
// BeginOnUIThread, GetFirstNonGeneratedView, ExecuteOnFirstLoad, ExecuteOnLayoutUpdated and
// GetViewCloseAction - a custom provider must satisfy every one of them, even though this
// exercise's own lesson is only about the first.

using Caliburn.Micro;
// IPlatformProvider's BeginOnUIThread/OnUIThread each take a plain System.Action - but
// "using Caliburn.Micro;" above also brings Caliburn.Micro.Action (the ActionMessage-related
// type) into scope, and both are arity-0 "Action", so the bare name is ambiguous (CS0104).
// Action<object> below is unaffected - Caliburn.Micro.Action has no generic form to collide with.
using SystemAction = System.Action;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A platform provider that reports design mode however it is constructed to,
/// regardless of what the wrapped real provider would say - every OTHER member simply forwards
/// to that real provider, because Execute still needs somewhere genuine to marshal work
/// through.</summary>
public class Ex064_FakeDesignModeProvider(IPlatformProvider inner, bool designMode) : IPlatformProvider
{
    /// <summary>THIS is the exercise: report the constructor's designMode flag, not
    /// inner.InDesignMode - the whole point is that this can disagree with what it wraps.</summary>
    public bool InDesignMode => designMode;

    public bool PropertyChangeNotificationsOnUIThread => inner.PropertyChangeNotificationsOnUIThread;
    public void BeginOnUIThread(SystemAction action) => inner.BeginOnUIThread(action);
    public Task OnUIThreadAsync(Func<Task> action) => inner.OnUIThreadAsync(action);
    public void OnUIThread(SystemAction action) => inner.OnUIThread(action);
    public object GetFirstNonGeneratedView(object view) => inner.GetFirstNonGeneratedView(view);
    public void ExecuteOnFirstLoad(object view, Action<object> action) => inner.ExecuteOnFirstLoad(view, action);
    public void ExecuteOnLayoutUpdated(object view, Action<object> action) => inner.ExecuteOnLayoutUpdated(view, action);

    public Func<CancellationToken, Task> GetViewCloseAction(object viewModel, ICollection<object> views, bool? dialogResult) =>
        inner.GetViewCloseAction(viewModel, views, dialogResult);
}

/// <summary>A view model showing canned sample data while Execute.InDesignMode is true, and its
/// real value otherwise - the classic reason design-time detection exists at all.</summary>
public class Ex064_DesignTimeAwareViewModel : PropertyChangedBase
{
    public const string SampleGreeting = "Sample greeting (design-time data)";

    /// <summary>The real value, as if it came from a live data source that only exists once the
    /// app is actually running.</summary>
    public string RealGreeting { get; set; } = "";

    /// <summary>Reads Execute.InDesignMode ITSELF - not a flag passed in at construction - so
    /// this answers correctly no matter which IPlatformProvider happens to be installed.</summary>
    public string Greeting => Execute.InDesignMode ? SampleGreeting : RealGreeting;
}
