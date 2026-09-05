// Exercise 035 - Conductor All Active (beginner).
// Goal:   Learn Conductor<T>.Collection.AllActive: every item in Items is active AT THE SAME
//         TIME - there is no single "current" item, and no ActiveItem property to ask for one.
// Drills: overriding CanCloseAsync to let an item refuse being closed, and OnDeactivateAsync to
//         record that closing one item never touches any of the others.
// Passes: dotnet test --filter FullyQualifiedName~Ex035_

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
        Task.FromResult(!RefuseClose);

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        DeactivateCount++;
        LastDeactivateWasClose = close;
        return Task.CompletedTask;
    }
}
