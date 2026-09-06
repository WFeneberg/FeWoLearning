using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 046 — SpanEnrichmentAndFiltering (web-services).
// Goal:   Add what the library could not know, and refuse what nobody wants, at the one
//         place that can do both cheaply.
// Drills: EnrichWithHttpRequest, the Filter predicate, dropping at the source.
// Passes: a served request carries the tenant taken from its own header, which no
//                     instrumentation library could have guessed at;
//         a request to /health produces NO span at all;
//         that /health request still succeeds - filtering removes the telemetry, not the
//                     behaviour;
//         and the filter is per request, so /orders in the same run is unaffected.
//
// The second clause is the cheapest performance win in this whole track. A liveness probe
// runs every few seconds per replica, forever; at fifty replicas that is a million spans a
// day describing nothing that ever varies. Dropping them in the collector still pays to
// build, serialise and ship them. Dropping them HERE means they are never created: the
// filter runs before the activity exists.
//
// The third clause is what stops that being reckless, and it is worth stating because
// "we turned off telemetry for /health" sounds like a step toward not knowing whether
// /health works. It is not: the endpoint still runs, still answers, and still fails
// loudly when it fails. What is gone is the per-invocation record of it having been
// boring.
//
// The first clause is the other half of row 041's division of labour, and this is where
// it goes in practice: not a span of your own alongside the framework's, but an extra
// attribute ON the framework's, so a single span carries both what the library knows and
// what only you do.
public static class Ex046_SpanEnrichmentAndFiltering
{
    /// <summary>The header a caller identifies its tenant with.</summary>
    public const string TenantHeader = "X-Tenant-Id";

    /// <summary>The attribute the tenant is recorded under.</summary>
    public const string TenantAttribute = "tenant.id";

    /// <summary>The route worth tracing.</summary>
    public const string OrdersRoute = "/orders/{id}";

    /// <summary>The route that is pure noise.</summary>
    public const string HealthRoute = "/health";

    /// <summary>
    /// Register tracing that records the ASP.NET Core instrumentation into
    /// <paramref name="exported"/>, and that:
    ///
    ///   - copies the <see cref="TenantHeader"/> of each request onto its span as
    ///     <see cref="TenantAttribute"/>, when the header is present;
    ///   - produces no span at all for requests whose path is
    ///     <see cref="HealthRoute"/>.
    /// </summary>
    public static void ConfigureTracing(IServiceCollection services, ICollection<Activity> exported) =>
        throw new NotImplementedException(
            "TODO: Ex046 - enrich the server span from the request, and filter the health probe out");

    /// <summary>Map <see cref="OrdersRoute"/> and <see cref="HealthRoute"/>.</summary>
    public static void MapEndpoints(IEndpointRouteBuilder endpoints) =>
        throw new NotImplementedException("TODO: Ex046 - map the orders endpoint and the health probe");
}
