// Exercise 033 - Conductor Single Active (beginner).
// Goal:   Learn Conductor<T>: a conductor that holds exactly one item at a time, and CLOSES
//         whichever item it replaces rather than merely setting it aside.
// Drills: overriding CanCloseAsync to let an item refuse being replaced, and OnDeactivateAsync
//         to record the close flag the conductor actually passed - close:true is the point of
//         this exercise, not just "some deactivation happened".
// Passes: dotnet test --filter FullyQualifiedName~Ex033_

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
        Task.FromResult(!RefuseClose);

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        DeactivateCount++;
        LastDeactivateWasClose = close;
        return Task.CompletedTask;
    }
}
