// Exercise 035 - Conductor All Active (beginner).
// Goal:   Learn Conductor<T>.Collection.AllActive: every item in Items is active AT THE SAME
//         TIME - there is no single "current" item, and no ActiveItem property to ask for one.
// Drills: overriding CanCloseAsync to let an item refuse being closed, and OnDeactivateAsync to
//         record that closing one item never touches any of the others.
// Passes: dotnet test --filter FullyQualifiedName~Ex035_
//
// Measured on this machine (Caliburn.Micro 5.0.258), on an ACTIVE AllActive conductor (activate
// it first, exactly as in ex033/ex034): activating two items leaves BOTH IsActive == true at
// once - activating a second item never deactivates the first, unlike Conductor<T> or OneActive.
// DeactivateItemAsync(item, close: true) still asks CanCloseAsync first, still removes only
// THAT item from Items, and never touches any other item's IsActive. Measured directly by
// reflection: Conductor<T>.Collection.AllActive has no ActiveItem property at all - it does not
// inherit ConductorBaseWithActiveItem<T> the way Conductor<T> and OneActive both do, because
// "the one active item" is not a concept AllActive has anything to offer.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex035_ConductorAllActive : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex035 - return !RefuseClose");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex035 - increment DeactivateCount and record close");
}
