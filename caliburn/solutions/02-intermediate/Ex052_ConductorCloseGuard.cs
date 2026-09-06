// Exercise 052 - Conductor Close Guard (intermediate).
// Goal:   A conductor's own CanCloseAsync is not a fact about the conductor itself - it is the
//         answer its children give, asked through its close strategy. One refusing child among
//         several willing ones is enough to make the CONDUCTOR'S CanCloseAsync refuse too.
//         CanCloseAsync is NOT a pure query in general, either: with the DEFAULT close strategy
//         used throughout this exercise, one refusal makes its Children come back empty, so
//         asking closes nothing - but that is a property of the DEFAULT strategy, not of the
//         guard itself. A strategy that returns a willing subset alongside CloseCanOccur == false
//         (ex053's flag, ex054's own majority strategy) makes this very same CanCloseAsync call
//         deactivate and remove those children, as a side effect of merely asking.
// Drills: writing a child's own CanCloseAsync override (the thing being cascaded), and writing
//         your OWN fold over Items that asks each child directly and ANDs the answers together -
//         the same fold the framework's own CanCloseAsync performs internally, but built here by
//         hand instead of just observed through the framework's result.
// Passes: dotnet test --filter FullyQualifiedName~Ex052_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A screen that can be told to refuse closing, and counts how many times it was asked.</summary>
public class Ex052_Child : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times CanCloseAsync actually ran.</summary>
    public int CanCloseAsyncCallCount { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
    {
        CanCloseAsyncCallCount++;
        return Task.FromResult(!RefuseClose);
    }
}

public class Ex052_ConductorCloseGuard : Conductor<Ex052_Child>.Collection.AllActive
{
    /// <summary>Asks every item currently in Items its own CanCloseAsync DIRECTLY - true only if
    /// EVERY one agrees, and every item must be asked (no short-circuiting on the first refusal).
    /// This is the same AND-over-children fold the framework's own CanCloseAsync performs through
    /// its close strategy - written here by hand so it is something you build, not just something
    /// you read about.</summary>
    public async Task<bool> AllChildrenWillingToCloseAsync()
    {
        var allWilling = true;
        foreach (var item in Items)
        {
            if (!await item.CanCloseAsync())
                allWilling = false;
        }
        return allWilling;
    }
}
