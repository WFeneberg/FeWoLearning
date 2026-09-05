using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex026;

/// <summary>Per-view state. Counts its own disposals, so "exactly once" is checkable.</summary>
public sealed class ViewLocalService : IDisposable
{
    public int DisposeCount { get; private set; }

    public void Dispose() => DisposeCount++;
}

/// <summary>Application-wide. Every view sees the same one.</summary>
public sealed class SharedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

// Exercise 026 — ScopedPerViewDi (desktop).
// Goal:   Give every open view its own DI scope, and close it deterministically when
//         the view goes away.
// Drills: child scopes, deterministic disposal, per-view lifetime.
// Passes: isolation   - two open views resolve DIFFERENT ViewLocalService instances.
//         consistency - one view resolves the SAME instance twice.
//         sharing     - both views see the same SharedService singleton.
//         closing     - CloseView disposes that view's service EXACTLY once.
//         the point   - closing one view does not dispose another view's service.
//         idempotence - closing twice does not dispose twice.
//         duplicates  - opening a view id that is already open is refused.
//
// A desktop application is a long-running process, which is what makes this different
// from a web request. Resolving view state straight from the root provider works
// perfectly for an afternoon and then the process is holding every view model, every
// cached image and every open file handle the user has opened since breakfast, because
// nothing ever told the container that a view had closed.
public sealed class ViewScopeManager(IServiceProvider root) : IDisposable
{
    /// <summary>Open a scope for <paramref name="viewId"/> and hand back its provider.</summary>
    public IServiceProvider OpenView(string viewId) =>
        throw new NotImplementedException(
            "TODO: Ex026 - create a child scope for this view id, refusing an id that is already open");

    /// <summary>Close the view's scope, disposing everything resolved inside it.</summary>
    public void CloseView(string viewId) =>
        throw new NotImplementedException(
            "TODO: Ex026 - dispose this view's scope exactly once, and ignore an id that is not open");

    public void Dispose() =>
        throw new NotImplementedException("TODO: Ex026 - close every remaining view");
}
