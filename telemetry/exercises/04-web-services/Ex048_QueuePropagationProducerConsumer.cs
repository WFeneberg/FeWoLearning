using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

/// <summary>A message on the queue: a body and whatever headers were put on it.</summary>
/// <param name="Body">What the message says.</param>
/// <param name="Headers">Where the trace context travels.</param>
public sealed record QueueMessage(string Body, IDictionary<string, string> Headers);

// Exercise 048 — QueuePropagationProducerConsumer (web-services).
// Goal:   Carry a trace across a hop that has no request, no response and no connection.
// Drills: Producer and Consumer kinds, injecting into message headers, extracting on the
//         other side, the messaging attributes.
// Passes: publishing produces a Producer span and a message carrying a traceparent
//                     header;
//         consuming that message produces a Consumer span on the SAME trace, whose
//                     parent is the producer's span and whose parent context is remote;
//         both spans carry the conventional messaging attributes;
//         and consuming a message with no header starts a NEW trace rather than failing.
//
// HTTP propagation happens by itself once an instrumentation library is registered.
// Queue propagation does not, and never will: a broker has no notion of a header your
// tracing library recognises, no request/response pair to hook, and often no connection
// at all between the two sides. Everything here is hand-written, and if you do not write
// it the trace simply ends at the publish - which reads, on every dashboard, as a system
// where nothing happens after an order is placed.
//
// The last clause is the case that arrives in production on day one: a message that was
// already in the queue before you shipped the propagation, or one from a producer you do
// not own. It has no context, and the correct answer is a fresh trace rather than an
// exception - an unlinked trace is a gap in a picture, a throwing consumer is a poison
// message that blocks the queue.
//
// Note the contrast with row 019. There the consumer took a BATCH and used links, because
// a span caused by twenty messages cannot honestly claim one parent. Here it takes ONE
// message, so parenthood is exactly right and links would be a weaker statement.
public static class Ex048_QueuePropagationProducerConsumer
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex048";

    /// <summary>The queue both sides name.</summary>
    public const string QueueName = "orders";

    /// <summary>The header the trace travels in.</summary>
    public const string TraceParentHeader = "traceparent";

    /// <summary>The conventional attribute naming the broker.</summary>
    public const string MessagingSystemAttribute = "messaging.system";

    /// <summary>The conventional attribute naming the queue.</summary>
    public const string MessagingDestinationAttribute = "messaging.destination.name";

    /// <summary>What this exercise's broker is called.</summary>
    public const string MessagingSystem = "fewolearning.queue";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        throw new NotImplementedException("TODO: Ex048 - build a provider recording this source");

    /// <summary>
    /// Publish <paramref name="body"/>: a span named "<c>orders publish</c>" of kind
    /// <see cref="ActivityKind.Producer"/>, carrying
    /// <see cref="MessagingSystemAttribute"/> and
    /// <see cref="MessagingDestinationAttribute"/>, with its OWN context written into
    /// the returned message's headers.
    /// </summary>
    public static QueueMessage Publish(string body) =>
        throw new NotImplementedException(
            "TODO: Ex048 - publish as a Producer and put this span's context in the message");

    /// <summary>
    /// Consume <paramref name="message"/>: a span named "<c>orders process</c>" of kind
    /// <see cref="ActivityKind.Consumer"/>, carrying the same two attributes, continuing
    /// whatever trace the headers describe.
    ///
    /// Start a fresh trace when they describe nothing. Return the span, stopped.
    /// </summary>
    public static Activity? Consume(QueueMessage message) =>
        throw new NotImplementedException(
            "TODO: Ex048 - consume as a Consumer, continuing the trace the headers carry");
}
