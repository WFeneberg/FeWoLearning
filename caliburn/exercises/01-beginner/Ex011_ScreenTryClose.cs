// Exercise 011 - Screen Try Close (beginner).
// Goal:   Learn that a screen does not close itself - something has to be conducting it.
// Drills: TryCloseAsync's dependence on Parent, and OnDeactivateAsync's close flag when a
//         close actually happens.
// Passes: dotnet test --filter FullyQualifiedName~Ex011_
//
// TryCloseAsync() is inherited, not overridden - there is nothing to implement on it
// directly. Measured behaviour: on a screen that has been activated but has NO Parent and
// no attached view, TryCloseAsync() is a silent no-op - it does not even consult
// CanCloseAsync, OnDeactivateAsync never runs, and IsActive stays true. Put the very same
// screen under an ACTIVE Conductor<T>.Collection.OneActive (ActivateItemAsync sets Parent),
// and TryCloseAsync() asks CanCloseAsync first: refused, the screen stays active and stays
// in the conductor's Items; allowed, OnDeactivateAsync(close: true) runs, IsActive goes
// false, and the conductor drops it from Items. A conductor only activates its children
// while the conductor itself is active - activate the conductor before ActivateItemAsync,
// or the child never truly activates at all.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex011_ScreenTryClose : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex011 - return !RefuseClose");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex011 - increment DeactivateCount and record close");
}
