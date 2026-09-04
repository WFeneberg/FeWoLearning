// Exercise 060 - Navigation Parameters (intermediate).
// Goal:   Carry data into a page, and get an answer back out of it.
// Drills: the parameter on Navigate reaching NavigationEventArgs.Parameter, typed
//         parameters over strings, and the back stack keeping the previous entry's
//         parameter so a return re-runs with it.
// Passes: dotnet test --filter FullyQualifiedName~Ex060_
//
// The parameter is typed as object, so anything compiles - and a page that assumes a string
// fails at runtime for the one caller that passed an id. Checking the type at the boundary
// is the whole discipline here. Note also what a back stack entry stores: the parameter,
// not the page - the page is rebuilt and told the old parameter again.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>What a caller hands to the detail page.</summary>
/// <remarks>
/// `partial` because Uno.Extensions.Reactive - referenced for the MVUX exercises - has a
/// source generator that offers IKeyEquatable to any record with an Id-shaped member, and
/// errors out (KE0001) on one that is not partial. Every record in this assembly is
/// therefore partial, whether or not it takes part in MVUX.
/// </remarks>
public sealed partial record Ex060_DetailRequest(int Id, string Title);

/// <summary>
/// A page that reads a typed request out of its navigation parameter.
/// </summary>
public sealed partial class Ex060_DetailPage : Page
{
    /// <summary>The requests this page has been opened with, in order.</summary>
    public static List<Ex060_DetailRequest> Received { get; } = [];

    /// <summary>How many times a parameter this page could not use arrived.</summary>
    public static int Rejected { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e) =>
        // TODO: read e.Parameter. A well-formed Ex060_DetailRequest is appended to
        // Received and shown as Content (a TextBlock with the request's Title); anything
        // else counts as Rejected and shows nothing - the page must not throw on a
        // parameter it did not expect.
        throw new NotImplementedException("TODO: Ex060 - accept a typed request, reject the rest");
}

public static class Ex060_NavigationParameters
{
    /// <summary>A Frame with nothing on it yet.</summary>
    public static Frame CreateFrame() => new();

    /// <summary>
    /// Opens the detail page for <paramref name="request"/> on <paramref name="frame"/>,
    /// returning whether the navigation was accepted.
    /// </summary>
    public static bool OpenDetail(Frame frame, Ex060_DetailRequest request) =>
        throw new NotImplementedException("TODO: Ex060 - navigate with the typed request");

    /// <summary>
    /// Goes back if there is anywhere to go, and reports whether it did. The Frame replays
    /// the previous entry's parameter, so the page arrives with its old request again.
    /// </summary>
    public static bool GoBackIfPossible(Frame frame) =>
        throw new NotImplementedException("TODO: Ex060 - go back when the stack allows it");
}
