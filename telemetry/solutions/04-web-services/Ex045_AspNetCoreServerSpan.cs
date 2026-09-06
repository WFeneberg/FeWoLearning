using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 045 — AspNetCoreServerSpan (web-services).
// Goal:   Get one span per request whose NAME is bounded, however many distinct URLs
//         arrive.
// Drills: AddAspNetCoreInstrumentation, the route template, http.route, unmatched
//         requests.
// Passes: a request to a mapped endpoint produces exactly one Server span;
//         its name is the METHOD plus the ROUTE TEMPLATE - "GET /orders/{id}" - not the
//                     path that was requested;
//         two requests for different ids produce the SAME span name and different
//                     url.path values;
//         and a request that matches no route produces a span with no http.route at all.
//
// The third clause is the whole row, and it is the same lesson as row 021's tags and row
// 033's views, arriving for the third time because it is the one that costs money. A
// span name is a low-cardinality field: backends group, index and chart on it. Name the
// span after the path and a service with a million orders has a million span names, which
// is not a slow query, it is a broken one - and the "operations" list in every tracing UI
// becomes unusable in a way nobody can undo after the fact.
//
// The template is bounded by the number of routes you wrote. The path is bounded by your
// customers.
//
// The fourth clause is the corner people forget. A 404 has no route, so there is nothing
// to name the span after and the instrumentation says so by leaving http.route unset
// rather than by inventing one. Anything that reads http.route has to cope with its
// absence - and a dashboard that groups by it silently drops every unmatched request,
// which is exactly the traffic worth looking at during an attack or a bad deploy.
public static class Ex045_AspNetCoreServerSpan
{
    /// <summary>The route the exercise serves.</summary>
    public const string OrdersRoute = "/orders/{id}";

    /// <summary>What the span for that route must be called.</summary>
    public const string OrdersSpanName = "GET /orders/{id}";

    /// <summary>The conventional attribute carrying the template.</summary>
    public const string HttpRouteTag = "http.route";

    /// <summary>The conventional attribute carrying what was actually requested.</summary>
    public const string UrlPathTag = "url.path";

    /// <summary>
    /// Register tracing that records the ASP.NET Core instrumentation's spans into
    /// <paramref name="exported"/>.
    /// </summary>
    public static void ConfigureTracing(IServiceCollection services, ICollection<Activity> exported) =>
        services.AddOpenTelemetry().WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddInMemoryExporter(exported));

    /// <summary>
    /// Map <see cref="OrdersRoute"/> as a GET endpoint returning a body naming the order.
    ///
    /// Map nothing else: an unmatched request is half of what this row grades.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder endpoints) =>
        // The TEMPLATE is what routing matches on, and it is what the instrumentation
        // names the span after. Nothing here has to ask for that.
        endpoints.MapGet(OrdersRoute, (string id) => $"order {id}");
}
