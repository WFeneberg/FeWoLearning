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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Arrivals++;

        // The Frame instantiated the type and set it as its Content - that is all it does.
        // Anything visible has to be built here or in the constructor.
        Content = new TextBlock { Text = "first" };
    }

    // The counterpart, and where a page unsubscribes from anything it hooked up on
    // arrival: a page left on the back stack is not disposed, it is simply not shown.
    protected override void OnNavigatedFrom(NavigationEventArgs e) => Departures++;
}

/// <summary>The second page, so there is somewhere to go.</summary>
public sealed partial class Ex059_SecondPage : Page
{
    public static int Arrivals { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Arrivals++;
        Content = new TextBlock { Text = "second" };
    }
}

public static class Ex059_FrameNavigation
{
    /// <summary>
    /// A Frame already showing <see cref="Ex059_FirstPage"/>, with an empty back stack.
    /// </summary>
    public static Frame CreateFrameOnFirstPage()
    {
        var frame = new Frame();

        // By Type, not by instance: the Frame owns the lifetime, which is what lets it
        // rebuild a page when the back stack replays an entry.
        frame.Navigate(typeof(Ex059_FirstPage));

        return frame;
    }
}
