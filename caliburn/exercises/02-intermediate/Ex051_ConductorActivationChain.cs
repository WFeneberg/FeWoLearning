// Exercise 051 - Conductor Activation Chain (intermediate).
// Goal:   Being a conductor's ActiveItem and being ACTIVE are two different things - a conductor
//         only propagates activation to its active item while the conductor itself is active.
//         ActivateItemAsync sets ActiveItem straight away, on an inactive conductor or not, but
//         the item's own OnActivatedAsync only runs once the conductor is (or becomes) active.
// Drills: reaching a conductor's own activation/deactivation through IActivate/IDeactivate -
//         explicit interface members, unreachable off a Conductor-typed reference without a cast -
//         and observing that deactivating the conductor cascades to its active child with
//         WHATEVER close flag was actually passed, not a hard-coded one.
// Passes: dotnet test --filter FullyQualifiedName~Ex051_
//
// Measured on this machine (Caliburn.Micro 5.0.258): ActivateItemAsync(a) on a conductor that is
// NOT itself active leaves a.IsActive == false and never runs a's OnActivatedAsync - but
// ActiveItem is already a, and a.Parent is already the conductor; being the ActiveItem and being
// active are set at two different times. Casting the conductor to IActivate and calling
// ActivateAsync afterwards activates BOTH the conductor and a in the same call - a.IsActive
// becomes true and OnActivatedAsync runs exactly once. Casting to IDeactivate and calling
// DeactivateAsync(close: false) afterwards deactivates a with that same close: false - not
// close: true, and ActiveItem is untouched by this, so reactivating the conductor afterwards
// reactivates the very same a again (OnActivatedAsync runs a second time) without ever calling
// SetActiveItemAsync again. Calling SetActiveItemAsync on a conductor that IS already active
// activates the incoming item immediately, in the same call - there is no separate "activate the
// conductor" step needed in that order.

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

public class Ex051_ConductorActivationChain : Conductor<Ex051_Child>
{
    /// <summary>Sets item as this conductor's ActiveItem via the framework's own ActivateItemAsync -
    /// deliberately nothing more. On an inactive conductor this sets ActiveItem/Parent without
    /// activating item; on an already-active conductor it activates item immediately.</summary>
    public Task SetActiveItemAsync(Ex051_Child item) =>
        throw new NotImplementedException("TODO: Ex051 - ActivateItemAsync(item)");

    /// <summary>Activates this conductor itself. IActivate is implemented EXPLICITLY, so there is
    /// no public ActivateAsync directly on a Conductor-typed reference - this needs a cast.</summary>
    public Task ActivateSelfAsync() =>
        throw new NotImplementedException("TODO: Ex051 - cast this to IActivate and call ActivateAsync");

    /// <summary>Deactivates this conductor itself with the given close flag - same explicit-interface
    /// story as ActivateSelfAsync, reached through IDeactivate instead.</summary>
    public Task DeactivateSelfAsync(bool close) =>
        throw new NotImplementedException("TODO: Ex051 - cast this to IDeactivate and call DeactivateAsync(close, ...)");
}

/// <summary>A screen that records its own activation/deactivation history - the child this
/// conductor's activation chain runs through.</summary>
public class Ex051_Child : Screen
{
    /// <summary>How many times OnActivatedAsync actually ran.</summary>
    public int ActivateCount { get; private set; }

    /// <summary>How many times OnDeactivateAsync actually ran.</summary>
    public int DeactivateCount { get; private set; }

    /// <summary>The close flag from the most recent OnDeactivateAsync call, if any.</summary>
    public bool? LastDeactivateWasClose { get; private set; }

    protected override Task OnActivatedAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex051 - increment ActivateCount and return a completed task");

    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Ex051 - increment DeactivateCount, record close, and return a completed task");
}
