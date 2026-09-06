namespace FeWoLearning.Architecture.Exercises.CrossCutting.Ex059;

public interface IBackend
{
    string Handle(string feature, string request);
}

/// <summary>Records everything it was asked to do, so "who served this" is checkable.</summary>
public sealed class RecordingBackend(string name) : IBackend
{
    public string Name => name;

    public List<string> Handled { get; } = [];

    public string Handle(string feature, string request)
    {
        Handled.Add(feature);
        return $"{name}:{feature}:{request}";
    }
}

// Exercise 059 — StranglerFigFacade (reference solution).
public sealed class StranglerFacade(IBackend legacy, IBackend replacement)
{
    // A SET of features, not a boolean. One flag is one cutover date and one very long
    // weekend - the big-bang rewrite this pattern exists to avoid.
    private readonly HashSet<string> _migrated = new(StringComparer.OrdinalIgnoreCase);

    public void Migrate(string feature) => _migrated.Add(feature);

    // The switch has to turn both ways. A facade that can only migrate forward is a
    // one-way door, and finding that out during an incident is finding out too late.
    public void Rollback(string feature) => _migrated.Remove(feature);

    public bool IsMigrated(string feature) => _migrated.Contains(feature);

    public string Route(string feature, string request) =>
        // Exactly one backend is called. Calling both - to compare, to warm a cache, to
        // "verify the migration" - doubles every side effect the feature has, and the
        // legacy system is usually the one with the side effects.
        IsMigrated(feature)
            ? replacement.Handle(feature, request)
            : legacy.Handle(feature, request);
}
