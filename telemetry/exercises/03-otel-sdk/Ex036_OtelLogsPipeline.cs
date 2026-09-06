using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 036 — OtelLogsPipeline (otel-sdk).
// Goal:   Send the ILogger the application already uses through the same SDK as its
//         traces and metrics, and see what a LogRecord actually holds.
// Drills: AddOpenTelemetry on ILoggingBuilder, LogRecord.Attributes, Body vs
//         FormattedMessage.
// Passes: the record's Attributes carry OrderId, City and {OriginalFormat}, with no
//                     opt-in of any kind;
//         Body is the message TEMPLATE, not the rendered sentence;
//         FormattedMessage is null by default and holds the rendered sentence only when
//                     the pipeline asked for it;
//         and two calls with different data share one Body while their attributes
//                     differ.
//
// The second and fourth clauses are the surprise, and they are good news. Everyone
// reads "Body" as "the message", and in an OTLP payload it is the field a viewer shows
// as the message - but what the SDK puts there is the constant template. So grouping,
// alerting and searching by event work on Body directly, and the thing that varies
// lives in Attributes where it can be queried.
//
// The third clause is the cost. Rendering the sentence is an allocation per record that
// nothing downstream needs - a backend can render it from Body and Attributes whenever
// a human looks - so the SDK does not do it unless you say so. Turning
// IncludeFormattedMessage on is reasonable when a sink cannot interpolate; turning it on
// "to be safe" is paying for a string nobody reads.
public static class Ex036_OtelLogsPipeline
{
    /// <summary>The category the exercise logs under.</summary>
    public const string CategoryName = "fewolearning.telemetry.ex036";

    /// <summary>The constant template every shipment record carries.</summary>
    public const string Template = "Order {OrderId} shipped to {City}";

    /// <summary>The event every shipment record carries.</summary>
    public static readonly EventId Shipped = new(3601, nameof(Shipped));

    /// <summary>
    /// Build an <see cref="ILoggerFactory"/> whose records go through the OpenTelemetry
    /// pipeline into <paramref name="exported"/>, capturing everything from Trace
    /// upwards.
    ///
    /// <paramref name="includeFormattedMessage"/> decides whether the rendered sentence
    /// is attached as well.
    ///
    /// The caller disposes it.
    /// </summary>
    public static ILoggerFactory Build(
        ICollection<LogRecord> exported, bool includeFormattedMessage = false) =>
        throw new NotImplementedException(
            "TODO: Ex036 - route ILogger through the OpenTelemetry logging pipeline into this exporter");

    /// <summary>
    /// Write ONE Information record using <see cref="Template"/> and
    /// <see cref="Shipped"/>.
    /// </summary>
    public static void LogShipped(ILogger logger, string orderId, string city) =>
        throw new NotImplementedException("TODO: Ex036 - log the shipment with the constant template");
}
