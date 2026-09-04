// Exercise 008 - Screen Initialize (beginner).
// Goal:   Run one-time setup exactly once, no matter how many times the screen is shown.
// Drills: OnInitializedAsync as a load-once hook, IsInitialized, and the Activated event's
//         WasInitialized flag telling a first activation apart from a reactivation.
// Passes: dotnet test --filter FullyQualifiedName~Ex008_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex008_ScreenInitialize : Screen
{
    /// <summary>How many times the one-time load actually ran.</summary>
    public int LoadCount { get; private set; }

    protected override Task OnInitializedAsync(CancellationToken cancellationToken)
    {
        LoadCount++;
        return Task.CompletedTask;
    }
}
