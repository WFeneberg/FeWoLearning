using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 042 — OtlpExporterConfiguration (otel-sdk). 🐳
// Goal:   Point the pipeline at something real, and let the operator do the pointing.
// Drills: OtlpExporterOptions, the OTEL_EXPORTER_OTLP_* variables, protocol and headers.
// Passes: with no environment set, the exporter falls back to its documented default
//                     endpoint;
//         OTEL_EXPORTER_OTLP_ENDPOINT moves it, with no code change;
//         OTEL_EXPORTER_OTLP_PROTOCOL and OTEL_EXPORTER_OTLP_HEADERS are read the same
//                     way, and the headers arrive parsed rather than as one string;
//         and 🐳 a real OpenTelemetry Collector receives a span this pipeline sent.
//
// The environment variables are not a convenience feature, they are the contract. The
// whole point of OTLP is that the application does not know what is collecting it: the
// same binary ships to a laptop with nothing listening, to a cluster with a sidecar
// collector, and to a vendor's endpoint behind an API key - and the only thing that
// differs is the environment it was started in. Hard-code the endpoint and you have
// turned a deployment decision into a build decision.
//
// The headers variable is where the API key goes, which is why its parsing matters: it
// is a comma-separated list of key=value pairs, and an exporter that treats it as one
// opaque string sends a header nobody accepts and gets a 401 with no explanation.
//
// The 🐳 fact is skipped unless the run passes -p:Containers=true. Everything above it
// is graded without Docker: the container proves the wire works, it does not carry the
// row.
public static class Ex042_OtlpExporterConfiguration
{
    /// <summary>Where to send it.</summary>
    public const string EndpointVariable = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary>How to encode it.</summary>
    public const string ProtocolVariable = "OTEL_EXPORTER_OTLP_PROTOCOL";

    /// <summary>What to send with it - an API key, usually.</summary>
    public const string HeadersVariable = "OTEL_EXPORTER_OTLP_HEADERS";

    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex042";

    /// <summary>What the exporter aims at when nothing says otherwise.</summary>
    public const string DefaultEndpoint = "http://localhost:4317/";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// The exporter configuration the environment currently dictates, with nothing set
    /// in code.
    /// </summary>
    public static OtlpExporterOptions FromEnvironment() =>
        throw new NotImplementedException(
            "TODO: Ex042 - hand back the options the OTEL_EXPORTER_OTLP_* variables produce");

    /// <summary>
    /// The headers from <see cref="HeadersVariable"/>, parsed into pairs.
    ///
    /// The variable is a comma-separated list of <c>key=value</c>. Return an empty
    /// dictionary when it is unset or empty.
    /// </summary>
    public static IReadOnlyDictionary<string, string> ParseHeaders(string? headersVariable) =>
        throw new NotImplementedException("TODO: Ex042 - parse the comma-separated header list");

    /// <summary>
    /// Build a provider exporting <see cref="SourceName"/> over OTLP/HTTP-protobuf to
    /// <paramref name="tracesEndpoint"/>, identifying itself as
    /// <paramref name="serviceName"/>.
    ///
    /// Use a SIMPLE export processor: a batch one would still be waiting when the test
    /// looks. The caller disposes the provider.
    /// </summary>
    public static TracerProvider BuildOtlp(Uri tracesEndpoint, string serviceName) =>
        throw new NotImplementedException("TODO: Ex042 - build an OTLP/HTTP pipeline aimed at that endpoint");

    /// <summary>Start and stop one span named <paramref name="name"/>.</summary>
    public static void DoWork(string name) =>
        throw new NotImplementedException("TODO: Ex042 - emit one span");
}
