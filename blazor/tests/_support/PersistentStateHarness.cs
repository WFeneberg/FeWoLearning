using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

namespace FeWoLearning.Blazor.Tests.Support;

/// <summary>
/// Stands in for the store a real Blazor host persists prerendered component state
/// into (the marker written into the HTML on the server, read back by the circuit).
/// Used by Ex059/Ex060, which are about the component half of that handover.
/// </summary>
public sealed class RecordingStateStore : IPersistentComponentStateStore
{
    private Dictionary<string, byte[]> _state = new();

    /// <summary>What the last <see cref="PersistStateAsync"/> wrote - the whole store,
    /// not a merge, exactly like the real one.</summary>
    public IReadOnlyDictionary<string, byte[]> Persisted => _state;

    public Task<IDictionary<string, byte[]>> GetPersistedStateAsync()
        => Task.FromResult<IDictionary<string, byte[]>>(new Dictionary<string, byte[]>(_state));

    public Task PersistStateAsync(IReadOnlyDictionary<string, byte[]> state)
    {
        _state = new Dictionary<string, byte[]>(state);
        return Task.CompletedTask;
    }

    public bool SupportsRenderMode(IComponentRenderMode renderMode) => true;
}

public static class PersistentStateHarness
{
    /// <summary>
    /// A fresh persistence manager - one per simulated render pass. Its
    /// <see cref="ComponentStatePersistenceManager.State"/> is what a component gets
    /// injected as <see cref="PersistentComponentState"/>.
    /// </summary>
    public static ComponentStatePersistenceManager CreateManager()
        => new(NullLogger<ComponentStatePersistenceManager>.Instance);
}
