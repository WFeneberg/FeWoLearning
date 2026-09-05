// Exercise 034 - Conductor One Active (beginner).
// Goal:   Learn Conductor<T>.Collection.OneActive: many items live in Items at once, but only
//         one of them is ever active - and unlike Conductor<T> (ex033), the outgoing item is
//         merely set aside, not closed.
// Drills: overriding CanCloseAsync to let an item refuse being closed, and OnDeactivateAsync to
//         record the close flag the conductor actually passed - close:false when it is simply
//         displaced by another item becoming active, close:true only when it is genuinely
//         removed from Items.
// Passes: dotnet test --filter FullyQualifiedName~Ex034_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex034_ConductorOneActive : Screen
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
