// Exercise 052 - Conductor Close Guard (intermediate).
// Goal:   A conductor's own CanCloseAsync is not a fact about the conductor itself - it is the
//         answer its children give, asked through its close strategy. One refusing child among
//         several willing ones is enough to make the CONDUCTOR'S CanCloseAsync refuse too, and
//         the guard is a pure query: asking it never closes or deactivates anyone by itself.
// Drills: writing a child's own CanCloseAsync override (the thing being cascaded), and wiring an
//         AllActive conductor so both children are genuinely active at once (unlike Conductor<T>'s
//         single-active replace semantics from the beginner tier).
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
    /// <summary>Activates this conductor (if it is not already), then activates BOTH first and
    /// second into it - AllActive keeps every item active simultaneously, unlike Conductor&lt;T&gt;'s
    /// single-active replace semantics.</summary>
    public async Task ActivateBothAsync(Ex052_Child first, Ex052_Child second)
    {
        if (!IsActive)
            await ((IActivate)this).ActivateAsync();
        await ActivateItemAsync(first);
        await ActivateItemAsync(second);
    }
}
