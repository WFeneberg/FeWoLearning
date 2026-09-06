using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex045_AspNetCoreServerSpanTests
{
    private static async Task<List<Activity>> Request(params string[] paths)
    {
        var exported = new List<Activity>();

        await using var web = await WebProbe.StartAsync(
            services => Ex045_AspNetCoreServerSpan.ConfigureTracing(services, exported),
            Ex045_AspNetCoreServerSpan.MapEndpoints);

        foreach (var path in paths) await web.Client.GetAsync(path);
        web.Services.GetRequiredService<TracerProvider>().ForceFlush();

        return exported;
    }

    [Fact]
    public async Task One_request_produces_exactly_one_server_span()
    {
        using var ctx = new TelemetryContext();

        var span = Assert.Single(await Request("/orders/42"));

        Assert.Equal(ActivityKind.Server, span.Kind);
    }

    [Fact]
    public async Task Adversarial_A_The_span_is_named_after_the_route_template()
    {
        // The whole row, and the third appearance of the lesson from rows 021 and 033
        // because it is the one that costs money. A span name is a low-cardinality field:
        // backends group, index and chart on it. Name it after the path and a service with
        // a million orders has a million span names - not a slow query, a broken one, and
        // the "operations" list in every tracing UI becomes unusable in a way nobody can
        // undo after the fact.
        using var ctx = new TelemetryContext();

        var span = Assert.Single(await Request("/orders/42"));

        Assert.Equal(Ex045_AspNetCoreServerSpan.OrdersSpanName, span.DisplayName);
        Assert.DoesNotContain("42", span.DisplayName);
        Assert.Equal(
            Ex045_AspNetCoreServerSpan.OrdersRoute,
            span.GetTagItem(Ex045_AspNetCoreServerSpan.HttpRouteTag)?.ToString());
    }

    [Fact]
    public async Task Adversarial_B_Different_ids_share_one_name_and_keep_their_own_path()
    {
        // The template is bounded by the number of routes you wrote. The path is bounded
        // by your customers. Both are recorded - one as the name, one as an attribute -
        // and which is which is the entire decision.
        using var ctx = new TelemetryContext();

        var spans = await Request("/orders/42", "/orders/99");

        Assert.Equal(2, spans.Count);
        Assert.Single(spans.Select(s => s.DisplayName).Distinct());
        Assert.Equal(
            ["/orders/42", "/orders/99"],
            spans.Select(s => s.GetTagItem(Ex045_AspNetCoreServerSpan.UrlPathTag)?.ToString()));
    }

    [Fact]
    public async Task Adversarial_C_An_unmatched_request_has_no_route_at_all()
    {
        // The corner people forget. A 404 has no route, so there is nothing to name the
        // span after and the instrumentation says so by leaving http.route unset rather
        // than inventing one.
        //
        // Anything reading http.route has to cope with its absence - and a dashboard that
        // groups by it silently drops every unmatched request, which is exactly the
        // traffic worth looking at during an attack or a bad deploy.
        using var ctx = new TelemetryContext();

        var span = Assert.Single(await Request("/nothing/here"));

        Assert.Null(span.GetTagItem(Ex045_AspNetCoreServerSpan.HttpRouteTag));
        Assert.NotEqual(Ex045_AspNetCoreServerSpan.OrdersSpanName, span.DisplayName);
    }

    [Fact]
    public async Task An_unmatched_request_is_still_recorded()
    {
        // The paired half. Dropping unmatched requests entirely would satisfy
        // Adversarial_C's first assertion and lose every 404 in the system.
        using var ctx = new TelemetryContext();

        var span = Assert.Single(await Request("/nothing/here"));

        Assert.Equal(ActivityKind.Server, span.Kind);
        Assert.Equal("/nothing/here", span.GetTagItem(Ex045_AspNetCoreServerSpan.UrlPathTag)?.ToString());
    }
}
