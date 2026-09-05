using FeWoLearning.Architecture.Exercises.Web.Ex016;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex016_HealthReadinessLivenessTests
{
    private sealed record Probe(string Name, bool IsDependency, HealthStatus Status) : IHealthCheck
    {
        public HealthStatus Check() => Status;
    }

    private static IReadOnlyList<IHealthCheck> Checks(
        HealthStatus database = HealthStatus.Healthy,
        HealthStatus broker = HealthStatus.Healthy,
        HealthStatus threadPool = HealthStatus.Healthy) =>
    [
        new Probe("database", IsDependency: true, database),
        new Probe("broker", IsDependency: true, broker),
        new Probe("thread-pool", IsDependency: false, threadPool),
    ];

    [Fact]
    public void All_Healthy_Passes_Both_Probes()
    {
        Assert.Equal(HealthStatus.Healthy, Ex016_HealthReadinessLiveness.Readiness(Checks()).Status);
        Assert.Equal(HealthStatus.Healthy, Ex016_HealthReadinessLiveness.Liveness(Checks()).Status);
    }

    [Fact]
    public void Mechanism_A_Failing_Dependency_Fails_Readiness_But_Not_Liveness()
    {
        // The fact this exercise exists for. One aggregate serving both probes is the
        // bug where a database blip restarts every instance of the service at once,
        // and none of them comes back any healthier.
        var checks = Checks(database: HealthStatus.Unhealthy);

        Assert.Equal(HealthStatus.Unhealthy, Ex016_HealthReadinessLiveness.Readiness(checks).Status);
        Assert.Equal(HealthStatus.Healthy, Ex016_HealthReadinessLiveness.Liveness(checks).Status);
    }

    [Fact]
    public void A_Failing_Process_Check_Fails_Both_Probes()
    {
        // Pairs with the fact above: liveness is not "always healthy", it is "healthy
        // unless the process itself is broken".
        var checks = Checks(threadPool: HealthStatus.Unhealthy);

        Assert.Equal(HealthStatus.Unhealthy, Ex016_HealthReadinessLiveness.Readiness(checks).Status);
        Assert.Equal(HealthStatus.Unhealthy, Ex016_HealthReadinessLiveness.Liveness(checks).Status);
    }

    [Fact]
    public void Adversarial_Aggregation_Keeps_Degraded_Distinct_From_Unhealthy()
    {
        // "Anything that is not Healthy is Unhealthy" passes every fact above and
        // throws away the state that says "still serving, stop sending NEW traffic"
        // rather than "restart me".
        var degraded = Ex016_HealthReadinessLiveness.Readiness(Checks(broker: HealthStatus.Degraded));
        Assert.Equal(HealthStatus.Degraded, degraded.Status);

        var worse = Ex016_HealthReadinessLiveness.Readiness(
            Checks(broker: HealthStatus.Degraded, database: HealthStatus.Unhealthy));
        Assert.Equal(HealthStatus.Unhealthy, worse.Status);
    }

    [Fact]
    public void Each_Probe_Reports_Only_The_Checks_It_Actually_Consulted()
    {
        // A liveness report listing the database is a liveness probe that consulted it -
        // whatever its aggregate happens to say today.
        var checks = Checks();

        Assert.Equal(["broker", "database", "thread-pool"],
            Ex016_HealthReadinessLiveness.Readiness(checks).Entries.Keys.OrderBy(k => k));

        Assert.Equal(["thread-pool"],
            Ex016_HealthReadinessLiveness.Liveness(checks).Entries.Keys);
    }
}
