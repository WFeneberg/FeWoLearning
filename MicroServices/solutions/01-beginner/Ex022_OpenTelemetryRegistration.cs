using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Get YOUR OWN spans and YOUR OWN metrics into the pipeline. Aspire has already
///         wired the exporter, the endpoint and the service name; none of that is the
///         part you write.
/// Drills: `WithTracing(t =&gt; t.AddSource(name))` and `WithMetrics(m =&gt; m.AddMeter(name))`.
///         An ActivitySource with no listener is not merely unexported - StartActivity
///         returns NULL and the span never exists. Registration is what creates the
///         listener, which is why "I added the ActivitySource" and "I registered it" are
///         two different statements.
/// Passes: With the registration in place, Orders.StartActivity("place-order") returns a
///         real Activity and an in-memory exporter receives it; OrdersPlaced.Add(...)
///         reaches the exporter as the metric "orders.placed". The Legacy source and the
///         Legacy meter, which nobody registered, reach neither.
/// Note:   Do NOT reach for OTEL_EXPORTER_OTLP_ENDPOINT or OTEL_SERVICE_NAME here. The
///         AppHost injects both into every resource on its own, so a test asserting them
///         grades Aspire and not the learner - which is exactly why catalog row 022 names
///         AddSource/AddMeter instead.
///
///         The plausible wrong answer that a positive-only test would accept is the
///         wildcard: AddSource("*") / AddMeter("*"). Measured on OpenTelemetry 1.18.0, a
///         wildcard registration exports the Legacy source AND the Legacy meter AND 18
///         System.Runtime metrics nobody asked for - so the row needs its negative half,
///         and the Legacy source and meter below exist only to provide it.
/// </summary>
public static class Ex022_OpenTelemetryRegistration
{
    // ---------------------------------------------------------------------------
    // GIVEN - the instruments themselves. Nothing here is a TODO: in a real service
    // these are the lines your domain code already has, and the exercise is about what
    // has to happen NEXT for them to be worth anything.
    // ---------------------------------------------------------------------------

    /// <summary>The source this exercise must register.</summary>
    public const string OrdersSourceName = "FeWoLearning.MicroServices.Orders";

    /// <summary>A second source, deliberately left unregistered. The negative half.</summary>
    public const string LegacySourceName = "FeWoLearning.MicroServices.Legacy";

    public static readonly ActivitySource Orders = new(OrdersSourceName);
    public static readonly ActivitySource Legacy = new(LegacySourceName);

    private static readonly Meter OrdersMeter = new(OrdersSourceName);
    private static readonly Meter LegacyMeter = new(LegacySourceName);

    public static readonly Counter<long> OrdersPlaced = OrdersMeter.CreateCounter<long>("orders.placed");
    public static readonly Counter<long> LegacyCalls = LegacyMeter.CreateCounter<long>("legacy.calls");

    // ---------------------------------------------------------------------------
    // The exercise
    // ---------------------------------------------------------------------------

    public static IHostApplicationBuilder AddOrdersTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(OrdersSourceName))
            .WithMetrics(metrics => metrics.AddMeter(OrdersSourceName));

        // Note what is NOT here: no exporter, no endpoint, no service name. Aspire
        // injects OTEL_EXPORTER_OTLP_ENDPOINT and OTEL_SERVICE_NAME into the resource,
        // and AddServiceDefaults (ex021) turns them into an OTLP exporter. The only
        // thing a service author owns is which of their own sources and meters are
        // allowed on the wire - and naming them exactly is the whole of the answer.
        return builder;
    }
}
