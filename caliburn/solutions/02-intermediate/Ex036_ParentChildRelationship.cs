// Exercise 036 - Parent Child Relationship (intermediate).
// Goal:   Learn IChild.Parent: the one-property escape hatch that lets a child reach the
//         conductor that owns it without ever being handed one - and that a conductor sets
//         this itself, the moment it activates an item into itself.
// Drills: IChild.Parent (Screen implements IChild), and IConductor.DeactivateItemAsync - asking
//         a conductor to close one of its own items from the CHILD's side of the relationship,
//         through Parent, rather than the conductor's own DeactivateItemAsync call the way
//         ex033/ex034 used it directly from the conductor's side.
// Passes: dotnet test --filter FullyQualifiedName~Ex036_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex036_ParentChildRelationship
{
    /// <summary>Returns the child's parent conductor, or null when it has no parent, or its parent is not an IConductor.</summary>
    public IConductor? GetParentConductor(IChild child) => child.Parent as IConductor;

    /// <summary>Asks the child's own parent conductor to close it - a no-op when the child has no parent conductor.</summary>
    public Task RequestCloseAsync(IChild child) =>
        GetParentConductor(child) is { } conductor
            ? conductor.DeactivateItemAsync(child, close: true)
            : Task.CompletedTask;
}

/// <summary>A screen that records how it was deactivated - used as a conductor's child in the tests.</summary>
public class Ex036_Child : Screen
{
    /// <summary>When true, CanCloseAsync refuses. Toggled directly by the test - no dialog.</summary>
    public bool RefuseClose { get; set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(!RefuseClose);

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        DeactivateCount++;
        return Task.CompletedTask;
    }
}
