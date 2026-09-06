namespace FeWoLearning.Architecture.Exercises.Scale.Ex069;

/// <summary>
/// One thing checked at startup. Required decides what happens when it is not there:
/// refuse to start, or start without it.
/// </summary>
public sealed record DependencyCheck(string Name, bool Required, Func<bool> Probe);

public sealed record StartupResult(bool Started, IReadOnlyList<string> Failed, IReadOnlyList<string> Degraded);

// Exercise 069 — StartupReadinessOrdering (reference solution).
public static class Ex069_StartupReadinessOrdering
{
    public static StartupResult Start(IReadOnlyList<DependencyCheck> checks)
    {
        ArgumentNullException.ThrowIfNull(checks);

        var failed = new List<string>();
        var degraded = new List<string>();

        // Every check, always. Returning at the first required failure turns diagnosing a
        // multi-dependency outage into one restart per dependency: "cannot reach the
        // database" sends an engineer to the database, while "cannot reach the database,
        // the broker and the identity provider" sends them to the network, which is where
        // the problem actually is.
        foreach (var check in checks)
        {
            if (check.Probe())
                continue;

            if (check.Required)
                failed.Add(check.Name);
            else
                degraded.Add(check.Name);
        }

        // Sorted, so the result does not depend on the order somebody happened to
        // register the checks in.
        failed.Sort(StringComparer.Ordinal);
        degraded.Sort(StringComparer.Ordinal);

        return new StartupResult(failed.Count == 0, failed, degraded);
    }
}
