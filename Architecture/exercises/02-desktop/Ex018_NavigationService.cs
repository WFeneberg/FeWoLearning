namespace FeWoLearning.Architecture.Exercises.Desktop.Ex018;

/// <summary>A view model that wants to know when it comes and goes.</summary>
public interface INavigationAware
{
    void OnNavigatedTo();
    void OnNavigatedFrom();
}

public sealed class PageViewModel(string name) : INavigationAware
{
    public string Name => name;

    /// <summary>Every lifecycle call this instance saw, in order.</summary>
    public List<string> Lifecycle { get; } = [];

    public void OnNavigatedTo() => Lifecycle.Add("to");

    public void OnNavigatedFrom() => Lifecycle.Add("from");
}

// Exercise 018 — NavigationService (desktop).
// Goal:   View-model-first navigation with a real back stack, and lifecycle callbacks
//         that fire in the right order on the right instances.
// Drills: view-model-first navigation, back stack, lifecycle callbacks.
// Passes: NavigateTo(a)        - Current is a, a saw "to", CanGoBack is false.
//         NavigateTo(b)        - Current is b; a saw "from" before b saw "to";
//                                CanGoBack is true.
//         GoBack()             - Current is a AGAIN - the same instance, not a new one -
//                                b saw "from", and a saw a second "to".
//         GoBack() with nothing behind it - throws InvalidOperationException.
//
// "The same instance" is the clause that matters. Going back is not navigating forward
// to a page that happens to look the same: the user expects their half-filled form,
// their scroll position and their selection to still be there. A service that
// reconstructs the view model passes every other assertion here and loses all of it.
public sealed class NavigationService
{
    public object? Current =>
        throw new NotImplementedException("TODO: Ex018 - the view model currently shown");

    public bool CanGoBack =>
        throw new NotImplementedException("TODO: Ex018 - whether there is anything behind Current");

    public void NavigateTo(object viewModel) =>
        throw new NotImplementedException(
            "TODO: Ex018 - push Current onto the back stack, tell it OnNavigatedFrom, then show the new one and tell it OnNavigatedTo");

    public void GoBack() =>
        throw new NotImplementedException(
            "TODO: Ex018 - restore the previous instance from the back stack, with the same from/to callbacks");
}
