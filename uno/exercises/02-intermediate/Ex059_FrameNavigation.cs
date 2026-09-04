// Exercise 059 - Frame Navigation (intermediate).
// Goal:   Move between pages and come back.
// Drills: Frame.Navigate/GoBack, OnNavigatedTo as the page's entry point, and the back
//         stack the Frame keeps for you.
// Passes: dotnet test --filter FullyQualifiedName~Ex059_
//
// A Page is a Control the Frame instantiates by Type - not an instance you hand over.
// Everything a page needs on arrival therefore comes through the navigation, which is why
// OnNavigatedTo exists and why a constructor is the wrong place to look at parameters.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A page that records its own navigation history for the test to inspect.
/// </summary>
public sealed partial class Ex059_FirstPage : Page
{
    /// <summary>How many times any instance of this page has been navigated to.</summary>
    public static int Arrivals { get; set; }

    /// <summary>How many times any instance has been navigated away from.</summary>
    public static int Departures { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e) =>
        // TODO: count the arrival and show something - set Content to a TextBlock reading
        // "first". A page navigated to must build its content here or in the constructor;
        // the Frame does nothing beyond instantiating the type.
        throw new NotImplementedException("TODO: Ex059 - handle arriving on the first page");

    protected override void OnNavigatedFrom(NavigationEventArgs e) =>
        throw new NotImplementedException("TODO: Ex059 - handle leaving the first page");
}

/// <summary>The second page, so there is somewhere to go.</summary>
public sealed partial class Ex059_SecondPage : Page
{
    public static int Arrivals { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e) =>
        throw new NotImplementedException("TODO: Ex059 - handle arriving on the second page");
}

public static class Ex059_FrameNavigation
{
    /// <summary>
    /// A Frame already showing <see cref="Ex059_FirstPage"/>, with an empty back stack.
    /// </summary>
    public static Frame CreateFrameOnFirstPage() =>
        // TODO: create the Frame and navigate it to the first page by Type.
        throw new NotImplementedException("TODO: Ex059 - open the frame on the first page");
}
