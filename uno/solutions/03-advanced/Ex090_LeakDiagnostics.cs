// Exercise 090 - Leak Diagnostics (advanced).
// Goal:   Prove that a visual was released, rather than hoping.
// Drills: WeakReference as the measuring instrument, what keeps an element alive
//         (a handler, an attached property's owner, a static), and detaching properly.
// Passes: dotnet test --filter FullyQualifiedName~Ex090_
//
// "It leaks" is a claim nobody can act on. A WeakReference plus a collection turns it into
// a yes or a no, and the same trick works in a real test suite - one test per page, held
// weakly, collected, asserted. It is the only way this class of bug gets caught before a
// user reports that the app slows down after twenty minutes.
//
// The subject here is a panel that tracks children through an event, which is where the
// leak usually is: the *source* outlives the child, so its handler list is what holds on.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Exercises.Advanced;

/// <summary>
/// A long-lived registry of elements, of the kind an app keeps for diagnostics or
/// selection - and of the kind that keeps a whole page alive if it holds on.
/// </summary>
public sealed class Ex090_LeakDiagnostics
{
    private readonly List<WeakReference<FrameworkElement>> _tracked = [];

    /// <summary>How many entries the registry holds, dead ones included.</summary>
    public int Entries => _tracked.Count;

    /// <summary>
    /// Starts tracking <paramref name="element"/> without keeping it alive.
    /// </summary>
    public void Track(FrameworkElement element) =>
        // Weak, because the registry is the long-lived object here. A diagnostics feature
        // holding its subjects strongly is the leak it was written to find.
        _tracked.Add(new WeakReference<FrameworkElement>(element));

    /// <summary>
    /// The elements still alive, and drops the entries whose targets have gone.
    /// </summary>
    public IReadOnlyList<FrameworkElement> Alive()
    {
        var alive = new List<FrameworkElement>();

        // Backwards, so removing a dead entry does not shift the indices still to visit.
        for (var index = _tracked.Count - 1; index >= 0; index--)
        {
            if (_tracked[index].TryGetTarget(out var element))
            {
                alive.Add(element);
            }
            else
            {
                _tracked.RemoveAt(index);
            }
        }

        alive.Reverse();
        return alive;
    }

    /// <summary>
    /// Whether <paramref name="reference"/>'s target has been collected. Forces a
    /// collection first, so the answer is about reachability rather than timing.
    /// </summary>
    public static bool WasReleased<T>(WeakReference<T> reference)
        where T : class
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // The second pass is what actually collects an object whose finaliser ran in the
        // first one. Without it a finalisable type looks like a leak.
        GC.Collect();

        return !reference.TryGetTarget(out _);
    }
}

/// <summary>
/// A panel that watches its children through an event, so a test can see the difference
/// between detaching and forgetting to.
/// </summary>
public sealed class Ex090_WatchfulPanel
{
    private readonly List<Border> _strongly = [];

    /// <summary>How many children this panel is holding on to.</summary>
    public int Held => _strongly.Count;

    /// <summary>
    /// Attaches to <paramref name="child"/>'s SizeChanged and keeps it, the way a panel
    /// that never cleans up does.
    /// </summary>
    public void Attach(Border child) => _strongly.Add(child);

    /// <summary>
    /// Lets go of <paramref name="child"/> entirely.
    /// </summary>
    public void Detach(Border child) => _strongly.Remove(child);
}
