using System.Net;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex052_HealthChecksAndProbesTests
{
    private static async Task<(HttpStatusCode Alive, HttpStatusCode Ready)> Probe(bool databaseIsReachable)
    {
        var previous = Ex052_HealthChecksAndProbes.DatabaseIsReachable;
        Ex052_HealthChecksAndProbes.DatabaseIsReachable = databaseIsReachable;

        try
        {
            await using var web = await WebProbe.StartAsync(
                Ex052_HealthChecksAndProbes.ConfigureHealthChecks,
                Ex052_HealthChecksAndProbes.MapProbes);

            var alive = await web.Client.GetAsync(Ex052_HealthChecksAndProbes.LivenessPath);
            var ready = await web.Client.GetAsync(Ex052_HealthChecksAndProbes.ReadinessPath);

            return (alive.StatusCode, ready.StatusCode);
        }
        finally
        {
            Ex052_HealthChecksAndProbes.DatabaseIsReachable = previous;
        }
    }

    [Fact]
    public async Task With_everything_up_both_probes_report_healthy()
    {
        var (alive, ready) = await Probe(databaseIsReachable: true);

        Assert.Equal(HttpStatusCode.OK, alive);
        Assert.Equal(HttpStatusCode.OK, ready);
    }

    [Fact]
    public async Task Adversarial_A_A_down_dependency_takes_readiness_and_leaves_liveness()
    {
        // The row, and getting it backwards is the outage that eats a cluster. Liveness
        // answers "is this process wedged - should you kill it". Readiness answers
        // "should traffic go here right now".
        //
        // A readiness check reporting live means an orchestrator restarts a perfectly
        // healthy process because its database is slow; every replica does the same thing
        // at the same time, none comes back faster for it, and the restart storm outlives
        // the original problem.
        var (alive, ready) = await Probe(databaseIsReachable: false);

        Assert.Equal(HttpStatusCode.OK, alive);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, ready);
    }

    [Fact]
    public async Task Adversarial_B_Readiness_recovers_on_its_own()
    {
        // The paired half. A readiness probe that latches after one failure turns a
        // three-second blip into a replica that never takes traffic again - which is
        // indistinguishable, from outside, from the dependency still being down.
        var down = await Probe(databaseIsReachable: false);
        var up = await Probe(databaseIsReachable: true);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, down.Ready);
        Assert.Equal(HttpStatusCode.OK, up.Ready);
    }

    [Fact]
    public async Task Adversarial_C_Each_endpoint_runs_only_its_own_tagged_checks()
    {
        // The mechanism that keeps the two apart: one registry of checks and two filtered
        // views of it, rather than two hand-maintained lists that drift the first time
        // somebody adds a check and forgets which endpoint should run it.
        //
        // If liveness ran everything, the fact above could not hold - so this is really
        // the same claim stated as a cause rather than an effect.
        var previous = Ex052_HealthChecksAndProbes.DatabaseIsReachable;
        Ex052_HealthChecksAndProbes.DatabaseIsReachable = false;

        try
        {
            await using var web = await WebProbe.StartAsync(
                Ex052_HealthChecksAndProbes.ConfigureHealthChecks,
                Ex052_HealthChecksAndProbes.MapProbes);

            var alive = await web.Client.GetAsync(Ex052_HealthChecksAndProbes.LivenessPath);
            var body = await alive.Content.ReadAsStringAsync();

            // The default writer reports the aggregate status only, so an unfiltered
            // liveness endpoint would have reported Unhealthy here.
            Assert.Equal(HttpStatusCode.OK, alive.StatusCode);
            Assert.Equal("Healthy", body);
        }
        finally
        {
            Ex052_HealthChecksAndProbes.DatabaseIsReachable = previous;
        }
    }
}
