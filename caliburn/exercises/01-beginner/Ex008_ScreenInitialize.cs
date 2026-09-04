// Exercise 008 - Screen Initialize (beginner).
// Goal:   Run one-time setup exactly once, no matter how many times the screen is shown.
// Drills: OnInitializedAsync as a load-once hook, IsInitialized, and the Activated event's
//         WasInitialized flag telling a first activation apart from a reactivation.
// Passes: dotnet test --filter FullyQualifiedName~Ex008_
//
// ActivateAsync calls OnInitializedAsync only on the very first activation, then never
// again - not on a second ActivateAsync while already active, and not after a
// deactivate/reactivate cycle. That is the whole point of the hook: it is where you load
// something expensive exactly once. Activated.WasInitialized is true only for that first
// call; every later activation carries WasInitialized == false, because OnInitializedAsync
// did not run for it. (Caliburn.Micro 5 deprecated OnInitializeAsync in favour of
// OnInitializedAsync - the old name still compiles and still runs, but is marked obsolete;
// override the -ed name.) ActivateAsync/DeactivateAsync themselves are reached through the
// IActivate/IDeactivate interfaces, never directly off a Screen-typed reference.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex008_ScreenInitialize : Screen
{
    /// <summary>How many times the one-time load actually ran.</summary>
    public int LoadCount { get; private set; }

    protected override Task OnInitializedAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex008 - increment LoadCount and return a completed task");
}
