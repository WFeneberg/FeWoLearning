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

// Exercise 026 — ScopedPerViewDi (reference solution).
public sealed class ViewScopeManager(IServiceProvider root) : IDisposable
{
    private readonly Dictionary<string, IServiceScope> _scopes = [];

    public IServiceProvider OpenView(string viewId)
    {
        if (_scopes.ContainsKey(viewId))
            throw new InvalidOperationException($"View '{viewId}' is already open.");

        // CreateScope, not the root provider. A scoped service resolved from the root
        // lives as long as the application does - which in a desktop process means
        // until the user closes it, holding every view model they have opened since.
        var scope = root.CreateScope();
        _scopes[viewId] = scope;
        return scope.ServiceProvider;
    }

    public void CloseView(string viewId)
    {
        // Remove BEFORE disposing, so a second close finds nothing and cannot dispose
        // the same scope twice.
        if (!_scopes.Remove(viewId, out var scope))
            return;

        scope.Dispose();
    }

    public void Dispose()
    {
        foreach (var scope in _scopes.Values)
            scope.Dispose();

        _scopes.Clear();
    }
}
