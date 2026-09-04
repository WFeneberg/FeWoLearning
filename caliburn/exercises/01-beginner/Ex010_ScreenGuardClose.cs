// Exercise 010 - Screen Guard Close (beginner).
// Goal:   Let a screen refuse to close by its own rules, asynchronously.
// Drills: overriding CanCloseAsync, and genuinely awaiting an async confirmation instead of
//         just wrapping a synchronous decision in an already-completed Task.
// Passes: dotnet test --filter FullyQualifiedName~Ex010_
//
// Screen.CanCloseAsync() returns true by default - nothing stops a fresh screen from
// closing. Overriding it lets a screen refuse: here, refuse while there are unsaved changes,
// unless a confirmation callback says it is fine to discard them. The confirmation must be
// genuinely awaited - not fired-and-ignored - and it must not be consulted at all when there
// is nothing to discard.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex010_ScreenGuardClose : Screen
{
    /// <summary>Whether the screen currently has changes that closing would discard.</summary>
    public bool HasUnsavedChanges { get; set; }

    /// <summary>
    /// Asked, and awaited, only when there are unsaved changes: true means "discard them and
    /// close anyway". Left null, a close with unsaved changes is refused outright.
    /// </summary>
    public Func<Task<bool>>? ConfirmDiscardAsync { get; set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex010 - allow when clean, otherwise await ConfirmDiscardAsync");
}
