using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 058 — MultiHopTraceCapstone (web-services).
// Goal:   Put rows 045, 048 and 049 together and get ONE trace across three hops that
//         share no call stack.
// Drills: the server span, injecting into a message, continuing it in a worker.
// Passes: one request produces three spans - the server's, the publish and the process;
//         all three share one trace id;
//         the chain is server → publish → process, each the parent of the next;
//         a message that arrives with no context still gets processed, on a trace of its
//                     own;
//         and the worker's span is a ROOT in that case rather than an orphan child.
//
// The fourth and fifth clauses are where this capstone says something the three rows it
// combines did not. Row 049 insisted that a worker's iterations be roots; this row has
// the worker continuing somebody else's trace, and both are right.
//
// The unit of work has not changed - it is still one item. What changed is whether a
// context arrived with it. A worker polling a queue nobody instrumented has no trace to
// join and starts its own; a worker handed a message carrying a traceparent continues the
// request that caused it. Same code, same rule: use the context you were given, and open
// a root when you were given none.
//
// What that buys is the thing this whole block has been building toward. A customer
// reports that an order never shipped; you have the trace id from their request; the
// worker's span is on it, an hour later, in a different process, with no HTTP connection
// between them - and the failure is right there.
public static class Ex058_MultiHopTraceCapstone
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex058";

    /// <summary>The route that accepts an order.</summary>
    public const string OrdersRoute = "/orders";

    /// <summary>The span the publish hop opens.</summary>
    public const string PublishSpanName = "orders publish";

    /// <summary>The span the worker hop opens.</summary>
    public const string ProcessSpanName = "orders process";

    /// <summary>The header the trace travels in.</summary>
    public const string TraceParentHeader = "traceparent";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Register tracing that records both the ASP.NET Core instrumentation and
    /// <see cref="SourceName"/> into <paramref name="exported"/>.
    /// </summary>
    public static void ConfigureTracing(IServiceCollection services, ICollection<Activity> exported) =>
        throw new NotImplementedException(
            "TODO: Ex058 - record the framework's server spans and this exercise's own");

    /// <summary>
    /// Map <see cref="OrdersRoute"/> as a POST-shaped GET endpoint that calls
    /// <see cref="Publish"/> and returns the message it produced, so the test can hand it
    /// to the worker.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder endpoints, IList<QueueMessage> queue) =>
        throw new NotImplementedException(
            "TODO: Ex058 - map the endpoint so a request publishes one message onto the queue");

    /// <summary>
    /// The publish hop: a <see cref="PublishSpanName"/> span of kind
    /// <see cref="ActivityKind.Producer"/>, with ITS OWN context written into the
    /// returned message's <see cref="TraceParentHeader"/>.
    /// </summary>
    public static QueueMessage Publish(string body) =>
        throw new NotImplementedException("TODO: Ex058 - publish, carrying this span's context");

    /// <summary>
    /// The worker hop: a <see cref="ProcessSpanName"/> span of kind
    /// <see cref="ActivityKind.Consumer"/>, continuing whatever trace the message carries.
    ///
    /// A message with no context gets a ROOT - not a child of whatever happened to be
    /// ambient in the worker. Returns the span, stopped.
    /// </summary>
    public static Activity? Process(QueueMessage message) =>
        throw new NotImplementedException(
            "TODO: Ex058 - continue the message's trace, or open a root when it carries none");
}
