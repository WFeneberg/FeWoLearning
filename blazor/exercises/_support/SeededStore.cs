using Microsoft.AspNetCore.Components;

namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for the PersistentComponentState exercises. Hands a
/// ComponentStatePersistenceManager a pre-seeded payload, exactly as a
/// prerender would, and records what a component persisted back.
/// Not an exercise.
/// </summary>
public sealed class SeededStore(IDictionary<string, byte[]> seed) : IPersistentComponentStateStore
{
    public IReadOnlyDictionary<string, byte[]>? Persisted { get; private set; }

    public Task<IDictionary<string, byte[]>> GetPersistedStateAsync()
        => Task.FromResult(seed);

    public Task PersistStateAsync(IReadOnlyDictionary<string, byte[]> state)
    {
        Persisted = state;
        return Task.CompletedTask;
    }
}
