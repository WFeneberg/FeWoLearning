using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 056 — CollectorPipeline (web-services). 🐳
// Goal:   Move a decision out of the application and into the thing between it and the
//         backend.
// Drills: an OTLP pipeline, an OpenTelemetry Collector config, an attributes processor.
// Passes: the application emits a span carrying an attribute it should not be storing;
//         the collector configuration declares a processor that deletes that attribute
//                     and wires it into the traces pipeline;
//         the same configuration keeps the span itself - it removes a field, not the
//                     data;
//         and 🐳 a real collector, given that configuration, receives the span and shows
//                     the span WITHOUT the attribute.
//
// Every fix in this track so far has been in the application: redact the field (row 009),
// bucket the route (row 050), keep the parameter out (row 051). All of them require a
// deployment of the service that is wrong - and if forty services are wrong, forty
// deployments, by forty teams, on forty schedules.
//
// A collector is one process that every service already sends to. A rule there applies to
// all of them at once, takes effect in the time it takes to restart one container, and
// can be written by whoever noticed the problem rather than by whoever owns the code.
//
// That is not an argument for doing everything there. A collector cannot un-send what has
// already left the process, cannot stop the cost of building the attribute, and is one
// more thing that can be misconfigured - so the application-side fix is still the right
// one for anything you know about in advance. What the collector buys is the ability to
// act on what you did NOT.
//
// The 🐳 fact is the only one that proves the config is real: everything above it grades
// artifacts, and a YAML document is only a claim until something parses it.
public static class Ex056_CollectorPipeline
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex056";

    /// <summary>The span the application emits.</summary>
    public const string SpanName = "checkout";

    /// <summary>The attribute that should never have been recorded.</summary>
    public const string SensitiveAttribute = "user.email";

    /// <summary>An attribute that must survive, so the fix is a filter rather than a drop.</summary>
    public const string OrderAttribute = "order.id";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Emit one <see cref="SpanName"/> span carrying <see cref="SensitiveAttribute"/> and
    /// <see cref="OrderAttribute"/>.
    ///
    /// This is the situation, not the mistake to fix here: the application records what
    /// it records, and the row is about what happens next.
    /// </summary>
    public static void DoCheckout(string email, string orderId)
    {
        using var activity = Source.StartActivity(SpanName);

        activity?.SetTag(SensitiveAttribute, email);
        activity?.SetTag(OrderAttribute, orderId);
    }

    /// <summary>
    /// Build a provider exporting <see cref="SourceName"/> over OTLP/HTTP-protobuf to
    /// <paramref name="tracesEndpoint"/> as <paramref name="serviceName"/>, with a SIMPLE
    /// export processor. The caller disposes it.
    /// </summary>
    public static TracerProvider BuildOtlp(Uri tracesEndpoint, string serviceName) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(serviceName))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = tracesEndpoint;
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.ExportProcessorType = ExportProcessorType.Simple;
            })
            .Build();

    /// <summary>
    /// The collector's configuration: an OTLP/HTTP receiver on
    /// <c>0.0.0.0:4318</c>, an <c>attributes</c> processor that DELETES
    /// <see cref="SensitiveAttribute"/>, the <c>debug</c> exporter at detailed verbosity,
    /// and a traces pipeline wiring all three together.
    ///
    /// Returned as YAML, because that is what the collector reads.
    /// </summary>
    public static string CollectorConfig() =>
        $"""
        receivers:
          otlp:
            protocols:
              http:
                endpoint: 0.0.0.0:4318
        processors:
          attributes:
            actions:
              - key: {SensitiveAttribute}
                action: delete
        exporters:
          debug:
            verbosity: detailed
        service:
          telemetry:
            logs:
              level: info
          pipelines:
            traces:
              receivers: [otlp]
              processors: [attributes]
              exporters: [debug]
        """;
}
