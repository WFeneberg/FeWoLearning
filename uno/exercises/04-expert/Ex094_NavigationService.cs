// Exercise 094 - Navigation Service (expert).
// Goal:   Let a view model navigate without knowing what a Frame is.
// Drills: an interface over Frame, a route table mapping names to page types, a typed
//         parameter, and a fake navigator so a view model can be tested at all.
// Passes: dotnet test --filter FullyQualifiedName~Ex094_
//
// A view model that calls Frame.Navigate(typeof(DetailPage)) needs a Frame to be tested,
// which means it needs a UI, which means it is not tested. One interface later the view
// model is a plain object again and the Frame lives behind an adapter nobody else sees.
//
// Route names rather than types are the second half: a feature module can register a route
// without every caller referencing its page class - which is what makes ex100 possible.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>What a view model is allowed to know about navigation.</summary>
public interface IEx094_Navigator
{
    /// <summary>Whether going back is possible.</summary>
    bool CanGoBack { get; }

    /// <summary>Navigates to a named route, optionally with a parameter.</summary>
    bool Navigate(string route, object? parameter = null);

    /// <summary>Goes back, and reports whether it did.</summary>
    bool GoBack();
}

/// <summary>A page the tests can navigate to.</summary>
public sealed partial class Ex094_DetailPage : Page
{
    /// <summary>The parameters this page has been opened with.</summary>
    public static List<object?> Received { get; } = [];

    protected override void OnNavigatedTo(NavigationEventArgs e) => Received.Add(e.Parameter);
}

/// <summary>A second page, so there is somewhere to go back from.</summary>
public sealed partial class Ex094_HomePage : Page
{
    public static int Arrivals { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e) => Arrivals++;
}

/// <summary>
/// The Uno-specific adapter: a route table over a real <see cref="Frame"/>. Everything a
/// view model touches is the interface above.
/// </summary>
public sealed class Ex094_FrameNavigator : IEx094_Navigator
{
    private readonly Frame _frame;
    private readonly Dictionary<string, Type> _routes;

    public Ex094_FrameNavigator(Frame frame, IReadOnlyDictionary<string, Type> routes)
    {
        _frame = frame;
        _routes = new Dictionary<string, Type>(routes, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanGoBack => _frame.CanGoBack;

    /// <summary>
    /// Navigates to <paramref name="route"/>. An unknown route is false, not an exception -
    /// a deep link or a stale button should not take the app down.
    /// </summary>
    public bool Navigate(string route, object? parameter = null) =>
        throw new NotImplementedException("TODO: Ex094 - resolve the route and navigate");

    public bool GoBack() =>
        throw new NotImplementedException("TODO: Ex094 - go back when the stack allows it");
}

/// <summary>A view model that navigates and can be tested without a Frame.</summary>
public sealed class Ex094_MenuViewModel
{
    private readonly IEx094_Navigator _navigator;

    public Ex094_MenuViewModel(IEx094_Navigator navigator) => _navigator = navigator;

    /// <summary>Opens the detail route for an id.</summary>
    public bool OpenDetail(int id) =>
        throw new NotImplementedException("TODO: Ex094 - navigate to the detail route");

    /// <summary>Goes back if it can.</summary>
    public bool Back() =>
        throw new NotImplementedException("TODO: Ex094 - go back through the navigator");
}
