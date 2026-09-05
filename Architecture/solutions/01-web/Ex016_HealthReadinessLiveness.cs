namespace FeWoLearning.Architecture.Exercises.Web.Ex016;

public enum HealthStatus
{
    Healthy = 0,
    Degraded = 1,
    Unhealthy = 2,
}

public interface IHealthCheck
{
    string Name { get; }

    /// <summary>
    /// True when this check reaches something OUTSIDE the process - a database, a
    /// broker, another service. That flag is the whole difference between the two
    /// probes.
    /// </summary>
    bool IsDependency { get; }

    HealthStatus Check();
}

public sealed record HealthReport(HealthStatus Status, IReadOnlyDictionary<string, HealthStatus> Entries);

// Exercise 016 — HealthReadinessLiveness (reference solution).
public static class Ex016_HealthReadinessLiveness
{
    public static HealthReport Readiness(IReadOnlyList<IHealthCheck> checks) =>
        Aggregate(checks);

    public static HealthReport Liveness(IReadOnlyList<IHealthCheck> checks) =>
        Aggregate([.. checks.Where(c => !c.IsDependency)]);

    private static HealthReport Aggregate(IReadOnlyList<IHealthCheck> checks)
    {
        var entries = checks.ToDictionary(c => c.Name, c => c.Check());

        // Max over the enum, whose values are ordered by severity. Collapsing to
        // "anything that is not Healthy is Unhealthy" throws away the Degraded state,
        // and Degraded is the one that says "still serving, stop sending new traffic"
        // rather than "restart me".
        var worst = entries.Count == 0
            ? HealthStatus.Healthy
            : entries.Values.Max();

        return new HealthReport(worst, entries);
    }
}
