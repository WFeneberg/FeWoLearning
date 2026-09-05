// Exercise 011 - Screen Try Close (beginner).
// Goal:   Learn that a screen does not close itself - something has to be conducting it.
// Drills: TryCloseAsync's dependence on Parent, and OnDeactivateAsync's close flag when a
//         close actually happens.
// Passes: dotnet test --filter FullyQualifiedName~Ex011_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex011_ScreenTryClose : Screen
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
