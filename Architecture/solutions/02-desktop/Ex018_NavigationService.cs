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

// Exercise 018 — NavigationService (reference solution).
public sealed class NavigationService
{
    private readonly Stack<object> _back = new();

    public object? Current { get; private set; }

    public bool CanGoBack => _back.Count > 0;

    public void NavigateTo(object viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (Current is not null)
        {
            (Current as INavigationAware)?.OnNavigatedFrom();
            // The INSTANCE goes on the stack, not its type or a factory for it. That is
            // what lets going back return the user's half-filled form rather than a
            // fresh one.
            _back.Push(Current);
        }

        Current = viewModel;
        (viewModel as INavigationAware)?.OnNavigatedTo();
    }

    public void GoBack()
    {
        if (!CanGoBack)
            throw new InvalidOperationException("There is nothing behind the current view model.");

        (Current as INavigationAware)?.OnNavigatedFrom();

        Current = _back.Pop();
        (Current as INavigationAware)?.OnNavigatedTo();
    }
}
