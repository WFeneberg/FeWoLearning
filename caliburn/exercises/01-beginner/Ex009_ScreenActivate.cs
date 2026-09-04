// Exercise 009 - Screen Activate (beginner).
// Goal:   Track a screen's active/inactive lifecycle, and know when the framework calls
//         your hooks at all.
// Drills: OnActivatedAsync / OnDeactivateAsync(close), IsActive, and the Activated /
//         Deactivated async events - including the two cases where nothing happens.
// Passes: dotnet test --filter FullyQualifiedName~Ex009_
//
// ActivateAsync on an already-active screen does nothing at all: no OnActivatedAsync call,
// no PropertyChanged, no Activated event. DeactivateAsync on a screen that was never
// activated is the same kind of no-op. Both hooks run on every REAL transition, and
// OnDeactivateAsync's close flag says whether the deactivation is a hide (close: false) or
// a genuine close (close: true) - your own state must remember which one happened.
// (Caliburn.Micro 5 deprecated OnInitializeAsync in favour of OnInitializedAsync, and
// OnActivateAsync in favour of OnActivatedAsync - the old names still compile and still
// run, but are marked obsolete; OnDeactivateAsync kept its original name and is not
// obsolete.)
// ActivateAsync/DeactivateAsync themselves are reached through the IActivate/IDeactivate
// interfaces, never directly off a Screen-typed reference.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex009_ScreenActivate : Screen
{
    /// <summary>How many times OnActivatedAsync actually ran.</summary>
    public int ActivateCount { get; private set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    protected override Task OnActivatedAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex009 - increment ActivateCount and return a completed task");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex009 - increment DeactivateCount, record close, and return a completed task");
}
