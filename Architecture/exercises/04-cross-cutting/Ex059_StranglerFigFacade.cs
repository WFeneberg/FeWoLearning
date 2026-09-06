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

// Exercise 059 — StranglerFigFacade (cross-cutting).
// Goal:   Replace a system one feature at a time, with both halves serving live traffic
//         at once and a single switch deciding which.
// Drills: routing facade, incremental cutover, per-feature migration state.
// Passes: migrated     - goes to the replacement, and the legacy backend is NOT called.
//         not migrated - goes to legacy, and the replacement is NOT called.
//         THE ONE       - with one feature migrated and one not, BOTH backends serve
//                        traffic in the same run. That is what "incremental" means.
//         at runtime    - migrating a feature changes routing immediately, with no
//                        restart; and un-migrating it rolls back the same way.
//         unknown       - a feature nobody has declared goes to legacy. The default is
//                        "leave it where it was".
//
// The both-at-once fact is the pattern. An all-or-nothing switch - one flag, one cutover
// date, one very long weekend - passes every single-feature assertion here, and is the
// big-bang rewrite this pattern exists to avoid. It is also why the rollback fact
// matters: the switch is only worth having if it turns both ways, and finding that out
// during an incident is finding out too late.
public sealed class StranglerFacade(IBackend legacy, IBackend replacement)
{
    /// <summary>Declare that <paramref name="feature"/> is now served by the replacement.</summary>
    public void Migrate(string feature) =>
        throw new NotImplementedException("TODO: Ex059 - mark this feature as migrated");

    /// <summary>Send it back to legacy.</summary>
    public void Rollback(string feature) =>
        throw new NotImplementedException("TODO: Ex059 - mark this feature as not migrated");

    public bool IsMigrated(string feature) =>
        throw new NotImplementedException("TODO: Ex059 - whether this feature is served by the replacement");

    /// <summary>Route one request to exactly one backend.</summary>
    public string Route(string feature, string request) =>
        throw new NotImplementedException(
            "TODO: Ex059 - send migrated features to the replacement and everything else to legacy, calling only one of them");
}
