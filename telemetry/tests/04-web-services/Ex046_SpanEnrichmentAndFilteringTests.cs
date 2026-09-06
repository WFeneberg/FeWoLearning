using System.Diagnostics;
using System.Net;
using System.Net.Http;
using FeWoLearning.Telemetry.Exercises.WebServices;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.WebServices;

public class Ex046_SpanEnrichmentAndFilteringTests
{
    private static async Task<(List<Activity> Spans, List<HttpStatusCode> Statuses)> Serve(
        params (string Path, string? Tenant)[] requests)
    {
        var exported = new List<Activity>();
        var statuses = new List<HttpStatusCode>();

        await using var web = await WebProbe.StartAsync(
            services => Ex046_SpanEnrichmentAndFiltering.ConfigureTracing(services, exported),
            Ex046_SpanEnrichmentAndFiltering.MapEndpoints);

        foreach (var (path, tenant) in requests)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (tenant is not null)
                request.Headers.Add(Ex046_SpanEnrichmentAndFiltering.TenantHeader, tenant);

            var response = await web.Client.SendAsync(request);
            statuses.Add(response.StatusCode);
        }

        web.Services.GetRequiredService<TracerProvider>().ForceFlush();

        return (exported, statuses);
    }

    [Fact]
    public async Task The_span_carries_the_tenant_from_the_requests_own_header()
    {
        // The other half of row 041's division of labour, in practice: not a span of your
        // own alongside the framework's, but an extra attribute ON the framework's, so one
        // span carries both what the library knows and what only you do.
        using var ctx = new TelemetryContext();

        var (spans, _) = await Serve(("/orders/42", "acme"));

        var span = Assert.Single(spans);
        Assert.Equal("acme", span.GetTagItem(Ex046_SpanEnrichmentAndFiltering.TenantAttribute)?.ToString());
    }

    [Fact]
    public async Task Adversarial_A_The_health_probe_produces_no_span_at_all()
    {
        // The cheapest performance win in this track. A liveness probe runs every few
        // seconds per replica, forever; at fifty replicas that is a million spans a day
        // describing nothing that ever varies. Dropping them in the collector still pays
        // to build, serialise and ship them - dropping them HERE means they are never
        // created, because the filter runs before the activity exists.
        using var ctx = new TelemetryContext();

        var (spans, _) = await Serve((Ex046_SpanEnrichmentAndFiltering.HealthRoute, null));

        Assert.Empty(spans);
    }

    [Fact]
    public async Task Adversarial_B_The_filtered_endpoint_still_works()
    {
        // What stops that being reckless. "We turned off telemetry for /health" sounds
        // like a step toward not knowing whether /health works. It is not: the endpoint
        // still runs, still answers, and still fails loudly when it fails. What is gone is
        // the per-invocation record of it having been boring.
        using var ctx = new TelemetryContext();

        var (_, statuses) = await Serve((Ex046_SpanEnrichmentAndFiltering.HealthRoute, null));

        Assert.Equal(HttpStatusCode.OK, Assert.Single(statuses));
    }

    [Fact]
    public async Task Adversarial_C_The_filter_is_per_request_not_a_global_switch()
    {
        // A filter written as "turn the instrumentation off" would satisfy Adversarial_A
        // perfectly and lose every span in the service.
        using var ctx = new TelemetryContext();

        var (spans, statuses) = await Serve(
            (Ex046_SpanEnrichmentAndFiltering.HealthRoute, null),
            ("/orders/42", "acme"),
            (Ex046_SpanEnrichmentAndFiltering.HealthRoute, null));

        Assert.Equal(3, statuses.Count);
        var span = Assert.Single(spans);
        Assert.Equal("/orders/42", span.GetTagItem("url.path")?.ToString());
    }

    [Fact]
    public async Task Adversarial_D_The_enrichment_reads_each_request_rather_than_a_constant()
    {
        // An enrichment hard-coded to one value satisfies the first fact and is useless.
        using var ctx = new TelemetryContext();

        var (spans, _) = await Serve(("/orders/1", "acme"), ("/orders/2", "globex"), ("/orders/3", null));

        Assert.Equal(3, spans.Count);

        // Looked up by path rather than by position. Measured 2026-09-06: the exported
        // order is NOT request order - GetAsync returns once the response headers arrive
        // while the server span ends a moment later, so two sequential requests can
        // interleave. A test that indexes into the export list is flaky, and flaky in the
        // direction that passes locally.
        string? TenantOf(string path) =>
            Assert.Single(spans, s => s.GetTagItem("url.path")?.ToString() == path)
                .GetTagItem(Ex046_SpanEnrichmentAndFiltering.TenantAttribute)?.ToString();

        Assert.Equal("acme", TenantOf("/orders/1"));
        Assert.Equal("globex", TenantOf("/orders/2"));

        // No header, no attribute - rather than an empty string nobody can filter out.
        Assert.Null(TenantOf("/orders/3"));
    }
}
