// Exercise 034 - Conductor One Active (beginner).
// Goal:   Learn Conductor<T>.Collection.OneActive from the inside: many items live in Items at
//         once, but only one of them is ever active - and unlike Conductor<T> (ex033), the
//         outgoing item is merely set aside, not closed, until it is explicitly closed.
// Drills: writing the conductor itself (this class IS a OneActive) - CloseActiveAsync must read
//         ActiveItem itself and hand it to DeactivateItemAsync with close:true, doing nothing
//         when there is no active item to close; overriding CanCloseAsync on the child to let
//         it refuse being closed, and OnDeactivateAsync to record the close flag the conductor
//         actually passed - close:false when merely displaced by another item becoming active,
//         close:true only when genuinely removed from Items.
// Passes: dotnet test --filter FullyQualifiedName~Ex034_
//
// Measured on this machine (Caliburn.Micro 5.0.258), on an ACTIVE OneActive conductor (activate
// it first, exactly as in ex033): ActivateItemAsync(c1) then ActivateItemAsync(c2) leaves BOTH
// in Items, ActiveItem becomes c2, and c1 receives OnDeactivateAsync(close: false) - the sharp
// contrast with Conductor<T>, which passes true for the very same kind of swap. Activating a
// third item only deactivates the second, never touching the first again - Items keeps growing,
// it never shrinks just from activating more items. DeactivateItemAsync(item, close: true) is
// the only thing that actually removes an item from Items - and it still asks CanCloseAsync
// first: a refusal leaves the item active and in Items exactly as it was. Once the active item
// is genuinely closed this way, the conductor promotes one of the remaining items to ActiveItem.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex034_ConductorOneActive : Conductor<Ex034_Child>.Collection.OneActive
{
    /// <summary>Closes whichever item is currently ActiveItem, if any - a no-op when nothing is active.</summary>
    public Task CloseActiveAsync() =>
        throw new NotImplementedException("TODO: Ex034 - if ActiveItem is not null, DeactivateItemAsync(ActiveItem, close: true)");
}

/// <summary>A screen that records how it was deactivated - one of this conductor's items.</summary>
public class Ex034_Child : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex034 - guard the close using RefuseClose");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex034 - increment DeactivateCount and record close");
}
