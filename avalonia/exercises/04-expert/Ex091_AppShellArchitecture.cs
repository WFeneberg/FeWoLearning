using System;
using System.Collections.Generic;
using ReactiveUI;
using Splat;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 091 - AppShellArchitecture (expert).
/// Goal:   Put the three things the earlier tiers taught separately into one
///         shell: a router that owns navigation, a container that builds the page
///         view models so they can take dependencies, and activation so a page
///         starts and stops work as it comes and goes.
/// Drills: IScreen with RoutingState, resolving a routable view model from a
///         container, IActivatableViewModel and WhenActivated, disposing on
///         deactivation.
/// Passes: dotnet test --filter FullyQualifiedName~Ex091_
///
/// The point of composing them is the seam between them, which is where real
/// shells go wrong:
///
///   - a page must be BUILT by the container, so it can be handed the services it
///     needs rather than newing them up and becoming untestable;
///   - the shell must not know what a page needs - it asks for a type and gets an
///     object back;
///   - and activation is NOT construction. A page navigated to twice is
///     constructed twice here, but the interesting number is how often it was
///     ACTIVATED and DEACTIVATED, because that is what starts and stops timers,
///     subscriptions and loads.
///
/// Measured, and worth knowing before you write the activation half: WhenActivated
/// runs when something actually activates the view model - in this exercise the
/// test does it explicitly through the given Activator, because there is no view
/// in the picture. Navigating alone does not activate anything.
public class Ex091_AppShellArchitecture : ReactiveObject, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new();

    /// <summary>Given. Do not change. The container the shell builds pages from.</summary>
    public ModernDependencyResolver Resolver { get; } = new();

    /// <summary>Given. Do not change. One entry per page the shell navigated to.</summary>
    public List<string> Visited { get; } = [];

    /// <summary>
    /// Register the two page view models on Resolver, each as a TRANSIENT so every
    /// navigation gets its own instance, and each taking this shell as its
    /// IScreen plus the given Ex091_Journal service.
    ///
    /// The journal itself is a LAZY SINGLETON: both pages write to the same one,
    /// which is what makes it worth injecting rather than constructing.
    /// </summary>
    public void Wire() =>
        throw new NotImplementedException(
            "TODO: Ex091 - a DependencyResolverRegistrar over Resolver: the journal " +
            "as a lazy singleton, and Ex091_HomeViewModel and Ex091_DetailViewModel " +
            "as transients built with (this, journal-from-the-resolver)");

    /// <summary>
    /// Resolve a page of type <typeparamref name="TPage"/> from the container,
    /// navigate to it, and append its UrlPathSegment to Visited.
    ///
    /// Resolve it rather than constructing it: a shell that news up its pages
    /// cannot be given a test double for anything they depend on.
    /// </summary>
    public TPage NavigateTo<TPage>()
        where TPage : class, IRoutableViewModel =>
        throw new NotImplementedException(
            "TODO: Ex091 - get a TPage from Resolver, throw InvalidOperationException " +
            "if it is not registered, push it onto Router with Navigate, and record " +
            "its UrlPathSegment in Visited");

    /// <summary>
    /// Go back one page, recording the segment of whatever is on top afterwards -
    /// or "(root)" when nothing is.
    /// </summary>
    public void GoBack() =>
        throw new NotImplementedException(
            "TODO: Ex091 - execute Router.NavigateBack, then append the current top " +
            "page's UrlPathSegment to Visited, or \"(root)\" when the stack is empty");
}

/// <summary>Given. Do not change. Shared by every page, hence a singleton.</summary>
public class Ex091_Journal
{
    public List<string> Entries { get; } = [];

    public void Write(string entry) => Entries.Add(entry);
}

/// <summary>
/// Given. Do not change. Note that neither page has a parameterless constructor:
/// they cannot be built without the container.
/// </summary>
public class Ex091_HomeViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public Ex091_HomeViewModel(IScreen hostScreen, Ex091_Journal journal)
    {
        HostScreen = hostScreen;
        Journal = journal;
        Activator = new ViewModelActivator();
    }

    public string? UrlPathSegment => "home";

    public IScreen HostScreen { get; }

    public Ex091_Journal Journal { get; }

    public ViewModelActivator Activator { get; }
}

/// <summary>Given. Do not change.</summary>
public class Ex091_DetailViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public Ex091_DetailViewModel(IScreen hostScreen, Ex091_Journal journal)
    {
        HostScreen = hostScreen;
        Journal = journal;
        Activator = new ViewModelActivator();
    }

    public string? UrlPathSegment => "detail";

    public IScreen HostScreen { get; }

    public Ex091_Journal Journal { get; }

    public ViewModelActivator Activator { get; }

    /// <summary>How many times this page has been activated.</summary>
    public int Activations { get; private set; }

    /// <summary>How many times it has been deactivated.</summary>
    public int Deactivations { get; private set; }

    /// <summary>Given. Do not change. Call these from your WhenActivated block.</summary>
    protected void CountActivation() => Activations++;

    /// <summary>Given. Do not change.</summary>
    protected void CountDeactivation() => Deactivations++;

    /// <summary>
    /// Set up activation: on every activation, count it and write
    /// "detail activated" to the Journal; when the activation ends, count the
    /// deactivation and write "detail deactivated".
    ///
    /// The disposal half is the part that is easy to leave out and impossible to
    /// notice until something leaks - a WhenActivated block that registers nothing
    /// for disposal never stops the work it started.
    /// </summary>
    public void SetUpActivation() =>
        throw new NotImplementedException(
            "TODO: Ex091 - this.WhenActivated((Action<IDisposable> register) => ...) " +
            "as ex048 did: count the activation and journal it, then register an " +
            "IDisposable that counts and journals the deactivation");
}
