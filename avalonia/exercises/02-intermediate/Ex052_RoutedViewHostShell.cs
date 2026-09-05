using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 052 - RoutedViewHostShell (intermediate).
/// Goal:   Wire a real RoutedViewHost - the "shell" - to a screen's Router and to
///         an explicit IViewLocator, so the host resolves and swaps its Content
///         on its own as the router navigates. Builds on ex050's explicit-locator
///         approach, one level up: there the caller resolved a single view; here
///         RoutedViewHost resolves repeatedly, on its own, across navigations.
/// Drills: RoutedViewHost, wiring Router + ViewLocator, IScreen, IRoutableViewModel.
///
/// Measured on this machine, with an explicit locator: navigate -> FooView,
/// navigate -> BarView, back -> FooView, and the locator was consulted exactly 3
/// times for those three navigations. RoutedViewHost exposes settable Router,
/// ViewLocator, DefaultContent and Content - Content is NOT part of this TODO;
/// only assign Router and ViewLocator and let the host resolve its own Content.
/// Passes: dotnet test --filter FullyQualifiedName~Ex052_
public class Ex052_ShellScreen : ReactiveObject, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new RoutingState();
}

/// <summary>Given. Do not change.</summary>
public class Ex052_FooViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "foo";
    public IScreen HostScreen { get; }
    public Ex052_FooViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

/// <summary>Given. Do not change.</summary>
public class Ex052_BarViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "bar";
    public IScreen HostScreen { get; }
    public Ex052_BarViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

/// <summary>Given. Do not change.</summary>
public class Ex052_FooView : UserControl, IViewFor<Ex052_FooViewModel>
{
    public Ex052_FooViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex052_FooViewModel?)value;
    }
}

/// <summary>Given. Do not change.</summary>
public class Ex052_BarView : UserControl, IViewFor<Ex052_BarViewModel>
{
    public Ex052_BarViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (Ex052_BarViewModel?)value;
    }
}

public class Ex052_Shell
{
    /// <summary>
    /// TODO: return a new RoutedViewHost with its Router set to screen.Router and
    /// its ViewLocator set to locator, so the host resolves its own Content as
    /// the router navigates. Do NOT resolve or assign Content yourself here -
    /// that is RoutedViewHost's job once it is wired, and the whole point of
    /// this exercise is to let it do that job rather than reimplementing it.
    /// </summary>
    public RoutedViewHost Build(IScreen screen, IViewLocator locator)
    {
        throw new NotImplementedException(
            "TODO: Ex052 - return new RoutedViewHost { Router = screen.Router, " +
            "ViewLocator = locator }");
    }
}
