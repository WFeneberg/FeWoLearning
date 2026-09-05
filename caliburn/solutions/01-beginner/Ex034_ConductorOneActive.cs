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

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex034_ConductorOneActive : Conductor<Ex034_Child>.Collection.OneActive
{
    /// <summary>Closes whichever item is currently ActiveItem, if any - a no-op when nothing is active.</summary>
    public Task CloseActiveAsync() =>
        ActiveItem is null ? Task.CompletedTask : DeactivateItemAsync(ActiveItem, close: true);
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
        Task.FromResult(!RefuseClose);

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        DeactivateCount++;
        LastDeactivateWasClose = close;
        return Task.CompletedTask;
    }
}
