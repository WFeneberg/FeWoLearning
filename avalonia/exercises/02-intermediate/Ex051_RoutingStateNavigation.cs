using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 051 - RoutingStateNavigation (intermediate).
/// Goal:   Drive ReactiveUI's RoutingState through a screen view model: push a
///         routable view model onto the stack via Navigate, and pop the most
///         recent one via NavigateBack.
/// Drills: RoutingState, IScreen, IRoutableViewModel, Navigate/NavigateBack.
///
/// Measured on this machine: RoutingState.CurrentViewModel is an IObservable, NOT
/// a synchronously-readable property - reading it directly yields the observable's
/// own type, not the current page. Subscribe to it (or read NavigationStack) to
/// see what is actually current. Router.Navigate and Router.NavigateBack are
/// ReactiveCommands, executed synchronously here with no scheduler needed:
/// `Router.Navigate.Execute(vm).Subscribe(_ => { }, _ => { })` - the two-arg
/// Subscribe absorbs the OnError a disallowed navigation (e.g. NavigateBack past
/// the root) raises, instead of letting it escape as an exception.
/// Passes: dotnet test --filter FullyQualifiedName~Ex051_
public class Ex051_FooViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Given. Do not change.</summary>
    public string? UrlPathSegment => "foo";

    /// <summary>Given. Do not change.</summary>
    public IScreen HostScreen { get; }

    public Ex051_FooViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

/// <summary>Given. Do not change.</summary>
public class Ex051_BarViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "bar";
    public IScreen HostScreen { get; }
    public Ex051_BarViewModel(IScreen hostScreen) => HostScreen = hostScreen;
}

public class Ex051_ScreenViewModel : ReactiveObject, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new RoutingState();

    /// <summary>
    /// TODO: push viewModel onto Router by executing Router.Navigate. A private
    /// stack field that never touches Router would leave Router.NavigationStack
    /// empty forever - the test reads NavigationStack directly, not a side channel.
    /// </summary>
    public void NavigateTo(IRoutableViewModel viewModel)
    {
        throw new NotImplementedException(
            "TODO: Ex051 - execute Router.Navigate.Execute(viewModel), subscribing " +
            "with a no-op onNext/onError");
    }

    /// <summary>
    /// TODO: pop the most recent view model by executing Router.NavigateBack.
    /// </summary>
    public void GoBack()
    {
        throw new NotImplementedException(
            "TODO: Ex051 - execute Router.NavigateBack.Execute(), subscribing " +
            "with a no-op onNext/onError so navigating back past the root does " +
            "not throw");
    }
}
