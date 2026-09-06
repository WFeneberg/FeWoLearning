using System.Collections.Generic;
using Avalonia.Controls;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 096 - MultiWindowLifetime (expert).
/// Goal:   Own a second window properly: open it against a parent, keep track of
///         it, and let it refuse to close - because "are you sure?" is a Closing
///         handler that cancels, not a dialog you hope the user reads.
/// Drills: Window.Show(owner), Window.Owner and OwnedWindows, Window.Closing with
///         CancelEventArgs, Window.IsVisible across a cancelled close.
/// Passes: dotnet test --filter FullyQualifiedName~Ex096_
///
/// WHAT THIS ROW DOES NOT USE, AND WHY THE CATALOG NOW SAYS SO. There is no
/// IClassicDesktopStyleApplicationLifetime here: measured,
/// Application.Current.ApplicationLifetime is NULL under the headless platform,
/// because a lifetime is something a platform head installs and this harness has
/// no head. So MainWindow, Shutdown and the ShutdownMode dance are all out of
/// reach, and anything in your own code that reads ApplicationLifetime has to
/// cope with null - which is worth knowing in itself, since the same is true in
/// a design-time context and in a unit test of any application.
///
/// What IS observable, all measured: Show(owner) sets Owner and adds the child to
/// the parent's OwnedWindows; a Closing handler that sets e.Cancel leaves the
/// child visible AND still owned; and a second Close then really closes it and
/// removes it from OwnedWindows.
public class Ex096_MultiWindowLifetime
{
    /// <summary>Given. Do not change. The parent window.</summary>
    public Window Shell { get; } = new() { Width = 200, Height = 140 };

    /// <summary>Given. Do not change. One entry per close attempt, in order.</summary>
    public List<string> CloseAttempts { get; } = [];

    /// <summary>Given. Do not change. True once the user has confirmed.</summary>
    public bool Confirmed { get; set; }

    /// <summary>The child, once Open has run.</summary>
    public Window? Tool { get; protected set; }

    /// <summary>
    /// Open a 120x90 tool window owned by Shell, and give it a Closing handler
    /// that appends "attempt" to CloseAttempts and CANCELS the close while
    /// Confirmed is false.
    ///
    /// Show it with the owner overload rather than the parameterless one: that is
    /// what makes it a child rather than a second top-level window, and it is what
    /// the OwnedWindows assertions turn on.
    /// </summary>
    public void Open() =>
        throw new NotImplementedException(
            "TODO: Ex096 - create a 120x90 Window, assign it to Tool, wire a Closing " +
            "handler that records the attempt and sets e.Cancel = !Confirmed, then " +
            "Show(Shell)");

    /// <summary>Ask the tool window to close. It may refuse.</summary>
    public void RequestClose() =>
        throw new NotImplementedException(
            "TODO: Ex096 - close Tool, if there is one");
}
