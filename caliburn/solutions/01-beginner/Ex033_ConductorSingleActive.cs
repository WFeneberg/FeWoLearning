// Exercise 033 - Conductor Single Active (beginner).
// Goal:   Learn Conductor<T> from the inside: a conductor that holds exactly one item at a
//         time, and CLOSES whichever item it replaces rather than merely setting it aside.
// Drills: writing the conductor itself (this class IS a Conductor<T>) - ShowAsync must activate
//         the conductor before it can activate a child, because a conductor only activates
//         children while it is itself active; overriding CanCloseAsync on the child to let it
//         refuse being replaced, and OnDeactivateAsync to record the close flag the conductor
//         actually passed - close:true is the point of this exercise, not just "some
//         deactivation happened".
// Passes: dotnet test --filter FullyQualifiedName~Ex033_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex033_ConductorSingleActive : Conductor<Ex033_Child>
{
    /// <summary>Activates this conductor first if it is not already active, then activates item into it - closing whatever item was shown before.</summary>
    public async Task ShowAsync(Ex033_Child item)
    {
        if (!IsActive)
            await ((IActivate)this).ActivateAsync();
        await ActivateItemAsync(item);
    }
}

/// <summary>A screen that records how it was deactivated - the item this conductor shows and replaces.</summary>
public class Ex033_Child : Screen
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
