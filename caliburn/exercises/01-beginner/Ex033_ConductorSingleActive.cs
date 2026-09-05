// Exercise 033 - Conductor Single Active (beginner).
// Goal:   Learn Conductor<T>: a conductor that holds exactly one item at a time, and CLOSES
//         whichever item it replaces rather than merely setting it aside.
// Drills: overriding CanCloseAsync to let an item refuse being replaced, and OnDeactivateAsync
//         to record the close flag the conductor actually passed - close:true is the point of
//         this exercise, not just "some deactivation happened".
// Passes: dotnet test --filter FullyQualifiedName~Ex033_
//
// Measured on this machine (Caliburn.Micro 5.0.258), on an ACTIVE Conductor<T> (a conductor
// only activates children while it is itself active - activate it first, or nothing below ever
// really happens): ActivateItemAsync(a) activates a and parents it to the conductor.
// ActivateItemAsync(b) activates b and sends a OnDeactivateAsync(close: true) - the item being
// REPLACED is closed, not just paused. Activating a third item afterwards closes only the item
// that was actually active at that moment - a is never touched again. If the current item's
// CanCloseAsync refuses, ActiveItem does not change at all: the incoming item is never
// activated, and the refuser is never deactivated.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex033_ConductorSingleActive : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex033 - return !RefuseClose");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex033 - increment DeactivateCount and record close");
}
