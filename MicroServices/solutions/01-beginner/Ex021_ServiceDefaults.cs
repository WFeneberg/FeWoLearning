using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Write the `AddServiceDefaults` extension every Aspire service calls on its
///         first line, and understand it as four independent registrations rather than
///         one magic word.
/// Drills: The four pillars of a cross-cutting service setup, each landing in the
///         IServiceCollection: OpenTelemetry (a TracerProvider and a MeterProvider),
///         health checks (a HealthCheckService plus a liveness-tagged "self" check),
///         service discovery (a ServiceEndpointResolver), and the STANDARD resilience
///         handler applied to the HttpClient DEFAULTS - i.e. to every client the
///         service will ever create, named or not.
/// Passes: A builder that has been through AddServiceDefaults resolves TracerProvider,
///         MeterProvider, HealthCheckService and ServiceEndpointResolver; carries an
///         IValidateOptions&lt;HttpStandardResilienceOptions&gt;, which only
///         AddStandardResilienceHandler registers; has TWO HttpMessageHandlerBuilder
///         actions waiting for a client name nobody has ever mentioned; and registers
///         exactly one health check, "self", tagged "live".
/// Note:   Do not grade "the app started" - that needs a socket and proves nothing about
///         which of the four pillars is missing. Grade the registrations. But a
///         registration assertion is easy to make vacuous, so this row measures a BARE
///         Host.CreateApplicationBuilder() in the same test first: none of the four types
///         above is there before AddServiceDefaults runs (measured on .NET 10.0.400 - a
///         bare builder holds 52 descriptors and not one of them is any of the four).
///         Watch out for what IS free: IMeterFactory is registered by the plain host
///         builder, and ActivitySource / DiagnosticListener / DistributedContextPropagator
///         by WebApplication.CreateBuilder, so an assertion built on those grades nothing.
///
///         Two mechanisms here look identical from a distance and are not. Resilience: a
///         hand-rolled AddResilienceHandler("...", pipeline =&gt; ...) registers no
///         HttpStandardResilienceOptions at all, which is how the test tells the standard
///         handler from a home-made one. Scope: AddHttpClient("catalog")
///         .AddStandardResilienceHandler() leaves the DEFAULT client options empty -
///         measured, 0 handler-builder actions for any other name - while
///         ConfigureHttpClientDefaults leaves 2 for every name, including names that do
///         not exist yet.
/// </summary>
public static class Ex021_ServiceDefaults
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Logs, traces and metrics are three separate pipelines that only share a
        // configuration object. Logging is wired on the ILoggingBuilder; the other two
        // on the service collection.
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());

        // Aspire injects OTEL_EXPORTER_OTLP_ENDPOINT itself, so this branch is dead in a
        // test and live under the AppHost. It is deliberately NOT graded - catalog row
        // 022 says why, and is the row about the part of OpenTelemetry that IS the
        // learner's: a custom ActivitySource and Meter.
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // "self" is the LIVENESS check: it answers "is this process still a process",
        // not "can it serve traffic yet". ex023 is the row about that distinction.
        builder.Services.AddHealthChecks()
               .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        // Resolves "https+http://catalog" to a real endpoint.
        builder.Services.AddServiceDiscovery();

        // DEFAULTS, not a named client: every HttpClient this service ever creates gets
        // both handlers, including clients registered after this line runs.
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }
}
