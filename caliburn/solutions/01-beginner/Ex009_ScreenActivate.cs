// Exercise 009 - Screen Activate (beginner).
// Goal:   Track a screen's active/inactive lifecycle, and know when the framework calls
//         your hooks at all.
// Drills: OnActivatedAsync / OnDeactivateAsync(close), IsActive, and the Activated /
//         Deactivated async events - including the two cases where nothing happens.
// Passes: dotnet test --filter FullyQualifiedName~Ex009_

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

    protected override Task OnActivatedAsync(CancellationToken cancellationToken)
    {
        ActivateCount++;
        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        DeactivateCount++;
        LastDeactivateWasClose = close;
        return Task.CompletedTask;
    }
}
