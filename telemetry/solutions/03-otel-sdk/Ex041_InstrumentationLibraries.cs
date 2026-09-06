using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 041 — InstrumentationLibraries (otel-sdk).
// Goal:   See exactly where the free telemetry stops and yours has to start.
// Drills: AddAspNetCoreInstrumentation, AddRuntimeInstrumentation, composing with your
//         own spans.
// Passes: with the instrumentation registered, a request produces the framework's Server
//                     span AND your own span, and yours is a CHILD of it;
//         without it, only your span exists, and it is a root;
//         the framework's span carries the conventional HTTP attributes and NOT your
//                     business attribute, which lives only on yours;
//         and runtime instrumentation publishes a pile of instruments nobody declared.
//
// The second clause is the one worth internalising. An instrumentation library is a
// REGISTRATION, not a property of the framework: ASP.NET Core does not "have tracing",
// it publishes events that something has to subscribe to. Forget the line and everything
// still runs, nothing errors, and your traces are simply missing their outermost span -
// which reads as "the request was fast" rather than as "nothing measured it".
//
// The third clause is the division of labour. A library can only record what is true of
// every application: a method, a route, a status code. It cannot know which tier the
// customer is on, and it cannot know that this request mattered more than the last one.
// The generic layer is free and the specific layer is yours, and a trace with only the
// first is a trace that can tell you a service is slow and never which requests.
//
// One thing this row deliberately does NOT cover, measured 2026-09-06:
// AddHttpClientInstrumentation cannot be exercised in-memory at all. The diagnostics
// handler it listens to is inserted by the real socket handler chain, so a client built
// over any custom handler - which is what an in-memory test server hands you - produces
// zero spans. That is a property of the transport, not of the instrumentation.
public static class Ex041_InstrumentationLibraries
{
    /// <summary>Your own source, alongside whatever the framework publishes.</summary>
    public const string OwnSourceName = "fewolearning.telemetry.ex041";

    /// <summary>The name of the span you start inside the handler.</summary>
    public const string OwnSpanName = "price.order";

    /// <summary>The attribute no library could ever have known to record.</summary>
    public const string TierTag = "order.tier";

    /// <summary>The route the exercise serves.</summary>
    public const string Route = "/orders/{id}";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Own { get; } = new(OwnSourceName);

    /// <summary>
    /// Register tracing into <paramref name="exported"/>, always listening to
    /// <see cref="OwnSourceName"/>, and adding the ASP.NET Core instrumentation only
    /// when <paramref name="withInstrumentation"/> says so.
    /// </summary>
    public static void ConfigureTracing(
        IServiceCollection services, ICollection<Activity> exported, bool withInstrumentation) =>
        services.AddOpenTelemetry().WithTracing(tracing =>
        {
            tracing.AddSource(OwnSourceName);

            // One line, and it is the whole difference between having an outermost span
            // and not. ASP.NET Core does not "have tracing" - it publishes events that
            // something has to subscribe to.
            if (withInstrumentation) tracing.AddAspNetCoreInstrumentation();

            tracing.AddInMemoryExporter(exported);
        });

    /// <summary>
    /// The endpoint handler. Start one <see cref="OwnSpanName"/> span, tag it
    /// <see cref="TierTag"/> with <paramref name="tier"/>, and return a body naming the
    /// order.
    /// </summary>
    public static string HandleOrder(string id, string tier)
    {
        // Nothing here correlates anything. The instrumentation's span is current while
        // the handler runs, so this one is nested simply by being started here.
        using var activity = Own.StartActivity(OwnSpanName);

        // The attribute no library could have known to record.
        activity?.SetTag(TierTag, tier);

        return $"order {id}";
    }

    /// <summary>
    /// Build a meter provider carrying the .NET runtime's own instrumentation into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static MeterProvider BuildRuntimeMetrics(ICollection<Metric> exported) =>
        Sdk.CreateMeterProviderBuilder()
            // No AddMeter call at all: the instrumentation library registers its own.
            .AddRuntimeInstrumentation()
            .AddInMemoryExporter(exported)
            .Build();
}
