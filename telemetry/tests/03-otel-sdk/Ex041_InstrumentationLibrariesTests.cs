using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Otel;
using FeWoLearning.Telemetry.Tests.Harness;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Tests.Otel;

public class Ex041_InstrumentationLibrariesTests
{
    private static async Task<List<Activity>> ServeOneRequest(bool withInstrumentation)
    {
        var exported = new List<Activity>();

        await using var web = await WebProbe.StartAsync(
            services => Ex041_InstrumentationLibraries.ConfigureTracing(services, exported, withInstrumentation),
            endpoints => endpoints.MapGet(
                Ex041_InstrumentationLibraries.Route,
                (string id) => Ex041_InstrumentationLibraries.HandleOrder(id, "gold")));

        await web.Client.GetAsync("/orders/42");
        web.Services.GetRequiredService<TracerProvider>().ForceFlush();

        return exported;
    }

    [Fact]
    public async Task The_framework_span_and_your_own_both_arrive()
    {
        using var ctx = new TelemetryContext();

        var spans = await ServeOneRequest(withInstrumentation: true);

        Assert.Contains(spans, s => s.Kind == ActivityKind.Server);
        Assert.Contains(spans, s => s.DisplayName == Ex041_InstrumentationLibraries.OwnSpanName);
    }

    [Fact]
    public async Task Your_span_is_a_child_of_the_frameworks()
    {
        // They compose with no correlation code at all: the instrumentation's span is
        // current while the handler runs, so anything started inside it is nested.
        using var ctx = new TelemetryContext();

        var spans = await ServeOneRequest(withInstrumentation: true);

        var server = Assert.Single(spans, s => s.Kind == ActivityKind.Server);
        var own = Assert.Single(spans, s => s.DisplayName == Ex041_InstrumentationLibraries.OwnSpanName);

        Assert.Equal(server.SpanId, own.ParentSpanId);
        Assert.Equal(server.TraceId, own.TraceId);
    }

    [Fact]
    public async Task Adversarial_A_Without_the_registration_there_is_no_framework_span()
    {
        // An instrumentation library is a REGISTRATION, not a property of the framework:
        // ASP.NET Core does not "have tracing", it publishes events that something has to
        // subscribe to. Forget the line and everything still runs, nothing errors, and
        // your traces are missing their outermost span - which reads as "the request was
        // fast" rather than "nothing measured it".
        using var ctx = new TelemetryContext();

        var spans = await ServeOneRequest(withInstrumentation: false);

        var own = Assert.Single(spans);
        Assert.Equal(Ex041_InstrumentationLibraries.OwnSpanName, own.DisplayName);
        Assert.Equal(default, own.ParentSpanId);
    }

    [Fact]
    public async Task Adversarial_B_The_business_attribute_is_only_on_your_span()
    {
        // The division of labour. A library can only record what is true of every
        // application - a method, a route, a status code. It cannot know which tier the
        // customer is on. A trace with only the generic layer tells you a service is slow
        // and never which requests.
        using var ctx = new TelemetryContext();

        var spans = await ServeOneRequest(withInstrumentation: true);

        var server = Assert.Single(spans, s => s.Kind == ActivityKind.Server);
        var own = Assert.Single(spans, s => s.DisplayName == Ex041_InstrumentationLibraries.OwnSpanName);

        Assert.Null(server.GetTagItem(Ex041_InstrumentationLibraries.TierTag));
        Assert.Equal("gold", own.GetTagItem(Ex041_InstrumentationLibraries.TierTag)?.ToString());

        // And the generic layer really is there, so this is a division rather than a gap.
        Assert.Equal("GET", server.GetTagItem("http.request.method")?.ToString());
    }

    [Fact]
    public void Adversarial_C_Runtime_instrumentation_publishes_instruments_nobody_declared()
    {
        // The same idea on the metrics side, and the clearest demonstration of what
        // "free" means: not one of these instruments appears anywhere in this repository.
        var exported = new List<Metric>();

        using (var provider = Ex041_InstrumentationLibraries.BuildRuntimeMetrics(exported))
        {
            provider.ForceFlush();
        }

        var names = MetricReadout.Of(exported).Select(p => p.Instrument).Distinct().ToArray();

        Assert.True(names.Length >= 5, $"expected the runtime's own instruments, saw {names.Length}");
        Assert.Contains(names, n => n.StartsWith("dotnet.", StringComparison.Ordinal));
    }
}
