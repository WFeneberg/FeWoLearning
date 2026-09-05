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

// Exercise 016 — HealthReadinessLiveness (web).
// Goal:   Compose health checks into two different probes that answer two different
//         questions, and aggregate their statuses honestly.
// Drills: health-check composition, readiness gating vs liveness.
// Passes: Readiness() - considers EVERY check; Entries names all of them.
//         Liveness()  - considers only the checks with IsDependency == false; Entries
//                       names only those.
//         the point   - a failing DEPENDENCY makes readiness Unhealthy and leaves
//                       liveness Healthy. A failing PROCESS check fails both.
//         aggregation - worst status wins, so Healthy + Degraded is Degraded, and
//                       Degraded + Unhealthy is Unhealthy - not "anything that is not
//                       Healthy is Unhealthy".
//
// The two probes are asked by different callers for different reasons. Readiness asks
// "should traffic be routed here right now"; liveness asks "is this process broken
// beyond recovery, restart it". Answering both with one aggregate is the bug where a
// database blip restarts every instance of the service at once, and none of them comes
// back up any healthier.
public static class Ex016_HealthReadinessLiveness
{
    public static HealthReport Readiness(IReadOnlyList<IHealthCheck> checks) =>
        throw new NotImplementedException(
            "TODO: Ex016 - aggregate every check, worst status wins");

    public static HealthReport Liveness(IReadOnlyList<IHealthCheck> checks) =>
        throw new NotImplementedException(
            "TODO: Ex016 - aggregate only the checks that are not dependencies, worst status wins");
}
