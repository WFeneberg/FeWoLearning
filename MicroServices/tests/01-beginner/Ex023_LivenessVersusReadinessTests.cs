using System.Net;
using FeWoLearning.MicroServices.Exercises.Beginner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex023_LivenessVersusReadinessTests
{
    /// <summary>
    /// Both probes, over a real HTTP round trip, with no socket: TestServer runs the
    /// whole routing and endpoint pipeline in memory. Status codes are the assertion
    /// because they are what an orchestrator actually reads - 200 keep me, 503 do
    /// something about me.
    /// </summary>
    private static async Task<(HttpStatusCode Alive, HttpStatusCode Health)> ProbeAsync(
        bool warmupComplete, bool eventLoopStalled, CancellationToken cancellationToken)
    {
        Ex023_LivenessVersusReadiness.WarmupComplete = warmupComplete;
        Ex023_LivenessVersusReadiness.EventLoopStalled = eventLoopStalled;

        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseTestServer();

        Ex023_LivenessVersusReadiness.ConfigureProbes(builder);

        await using var app = builder.Build();
        Ex023_LivenessVersusReadiness.MapProbes(app);

        await app.StartAsync(cancellationToken);
        var client = app.GetTestClient();

        var alive = (await client.GetAsync("/alive", cancellationToken)).StatusCode;
        var health = (await client.GetAsync("/health", cancellationToken)).StatusCode;

        await app.StopAsync(cancellationToken);
        return (alive, health);
    }

    [Fact]
    public async Task During_startup_the_service_is_live_but_not_ready()
    {
        var (alive, health) = await ProbeAsync(
            warmupComplete: false, eventLoopStalled: false, TestContext.Current.CancellationToken);

        // THE bug this row drills. A readiness check answering the liveness probe makes
        // a slow start look like a broken process, and the orchestrator restarts a
        // container that was going to be fine in four seconds - forever. Measured: an
        // "/alive" mapped with no predicate lands 503 right here.
        Assert.Equal(HttpStatusCode.OK, alive);

        // And the other half: readiness must NOT lie during startup either. A "/health"
        // that reports 200 while the database is still warming up sends traffic to an
        // instance that cannot serve it.
        Assert.Equal(HttpStatusCode.ServiceUnavailable, health);
    }

    [Fact]
    public async Task Once_warm_both_probes_report_healthy()
    {
        var (alive, health) = await ProbeAsync(
            warmupComplete: true, eventLoopStalled: false, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, alive);
        Assert.Equal(HttpStatusCode.OK, health);
    }

    [Fact]
    public async Task A_stalled_process_takes_BOTH_probes_down()
    {
        var (alive, health) = await ProbeAsync(
            warmupComplete: true, eventLoopStalled: true, TestContext.Current.CancellationToken);

        // The scenario that stops the filter being faked, and the only one that separates
        // the right answer from three plausible wrong ones. Measured, each of these
        // passes the two facts above and fails here:
        //   - "/alive" filtered by NAME (r => r.Name == "self") instead of by tag: it
        //     never runs "event-loop", so a wedged process reports 200 forever;
        //   - "/alive" as a MapGet(...) returning a constant 200: same symptom;
        //   - "/health" narrowed to the "ready" tag: a dead process reports ready.
        Assert.Equal(HttpStatusCode.ServiceUnavailable, alive);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, health);
    }

    [Fact]
    public async Task The_three_checks_carry_the_tags_the_endpoints_filter_on()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        Ex023_LivenessVersusReadiness.ConfigureProbes(builder);
        await using var app = builder.Build();

        var registrations = app.Services
            .GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations
            .ToDictionary(r => r.Name, r => r.Tags.Order().ToArray());

        // The registration side of the same claim, stated separately so a solution that
        // gets the right status codes out of hand-written predicates - and leaves the
        // tags off - still fails. The tags ARE the contract: they are what the next
        // check somebody adds will be classified by.
        Assert.Equal(["catalog-db", "event-loop", "self"], registrations.Keys.Order());
        Assert.Equal(["live"], registrations["self"]);
        Assert.Equal(["live"], registrations["event-loop"]);
        Assert.Equal(["ready"], registrations["catalog-db"]);
    }
}
