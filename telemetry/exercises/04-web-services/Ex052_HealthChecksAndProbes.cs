using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 052 — HealthChecksAndProbes (web-services).
// Goal:   Answer two different questions with two different endpoints, because an
//         orchestrator asks them for two different reasons.
// Drills: tagged health checks, tag-filtered endpoints, liveness versus readiness.
// Passes: /alive reports healthy while a dependency is down, because liveness does not
//                     depend on dependencies;
//         /ready reports unhealthy at the same moment, because readiness does;
//         both recover when the dependency comes back;
//         and each endpoint runs ONLY the checks carrying its own tag.
//
// The first two clauses together are the row, and getting them backwards is the outage
// that eats a cluster. Liveness answers "is this process wedged - should you kill it".
// Readiness answers "should traffic go here right now". A readiness check that reports
// live means an orchestrator restarts a perfectly healthy process because its database
// is slow; every replica does the same thing at the same time, none of them comes back
// faster for it, and the restart storm outlives the original problem.
//
// The rule that falls out: liveness must not touch anything it does not own. No
// database, no cache, no downstream service, no network at all if you can manage it.
// If the check can fail for a reason a restart cannot fix, it is not a liveness check.
//
// The fourth clause is the mechanism that keeps them apart. One registry of checks, two
// filtered views of it - not two hand-maintained lists that drift apart the first time
// somebody adds a check and forgets which endpoint should run it.
public static class Ex052_HealthChecksAndProbes
{
    /// <summary>The tag on checks a liveness probe may run.</summary>
    public const string LiveTag = "live";

    /// <summary>The tag on checks a readiness probe may run.</summary>
    public const string ReadyTag = "ready";

    /// <summary>Answers "is this process wedged".</summary>
    public const string LivenessPath = "/alive";

    /// <summary>Answers "should traffic come here".</summary>
    public const string ReadinessPath = "/ready";

    /// <summary>The name of the check that only looks inward.</summary>
    public const string SelfCheckName = "self";

    /// <summary>The name of the check that looks at a dependency.</summary>
    public const string DatabaseCheckName = "database";

    /// <summary>
    /// Whether the pretend dependency is currently reachable. The test moves this.
    /// </summary>
    public static bool DatabaseIsReachable { get; set; } = true;

    /// <summary>
    /// Register two health checks:
    ///
    ///   <see cref="SelfCheckName"/>, tagged <see cref="LiveTag"/>, always healthy -
    ///     it only reports that this process is running;
    ///   <see cref="DatabaseCheckName"/>, tagged <see cref="ReadyTag"/>, healthy exactly
    ///     when <see cref="DatabaseIsReachable"/> is true.
    /// </summary>
    public static void ConfigureHealthChecks(IServiceCollection services) =>
        throw new NotImplementedException(
            "TODO: Ex052 - register a self check tagged live and a dependency check tagged ready");

    /// <summary>
    /// Map <see cref="LivenessPath"/> and <see cref="ReadinessPath"/> so that each runs
    /// only the checks carrying its own tag.
    /// </summary>
    public static void MapProbes(IEndpointRouteBuilder endpoints) =>
        throw new NotImplementedException(
            "TODO: Ex052 - map two probes, each filtered to its own tag");

    /// <summary>
    /// The result the dependency check should report. Provided so the check itself stays
    /// a one-liner and the decision is visible.
    /// </summary>
    public static HealthCheckResult DatabaseResult() =>
        DatabaseIsReachable
            ? HealthCheckResult.Healthy("reachable")
            : HealthCheckResult.Unhealthy("unreachable");
}
