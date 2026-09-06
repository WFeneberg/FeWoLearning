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
        services.AddOpenTelemetry().WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddSource(SourceName)
            .AddInMemoryExporter(exported));

    /// <summary>
    /// Map <see cref="OrdersRoute"/> as a POST-shaped GET endpoint that calls
    /// <see cref="Publish"/> and returns the message it produced, so the test can hand it
    /// to the worker.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder endpoints, IList<QueueMessage> queue) =>
        endpoints.MapGet(OrdersRoute, () =>
        {
            // Publishing INSIDE the request is what makes the publish span a child of the
            // server span - no correlation code, just the ambient context.
            queue.Add(Publish("order placed"));

            return "accepted";
        });

    /// <summary>
    /// The publish hop: a <see cref="PublishSpanName"/> span of kind
    /// <see cref="ActivityKind.Producer"/>, with ITS OWN context written into the
    /// returned message's <see cref="TraceParentHeader"/>.
    /// </summary>
    public static QueueMessage Publish(string body)
    {
        var headers = new Dictionary<string, string>();

        using var activity = Source.StartActivity(PublishSpanName, ActivityKind.Producer);

        if (activity is not null)
        {
            headers[TraceParentHeader] =
                $"00-{activity.TraceId.ToHexString()}-{activity.SpanId.ToHexString()}-"
                + ((activity.ActivityTraceFlags & ActivityTraceFlags.Recorded) != 0 ? "01" : "00");
        }

        return new QueueMessage(body, headers);
    }

    /// <summary>
    /// The worker hop: a <see cref="ProcessSpanName"/> span of kind
    /// <see cref="ActivityKind.Consumer"/>, continuing whatever trace the message carries.
    ///
    /// A message with no context gets a ROOT - not a child of whatever happened to be
    /// ambient in the worker. Returns the span, stopped.
    /// </summary>
    public static Activity? Process(QueueMessage message)
    {
        // isRemote: true matters, and parsing twice would lose it - ActivityContext.Parse
        // has no such parameter, so the context from TryParse is the one to use.
        if (message.Headers.TryGetValue(TraceParentHeader, out var traceParent)
            && ActivityContext.TryParse(traceParent, null, isRemote: true, out var parent))
        {
            using var continued = Source.StartActivity(ProcessSpanName, ActivityKind.Consumer, parent);

            return continued;
        }

        // No context arrived, so there is no trace to join - and row 049's rule applies:
        // clear Activity.Current, because `parentContext: default` would inherit whatever
        // the worker happened to have open.
        var ambient = Activity.Current;
        Activity.Current = null;

        try
        {
            using var rooted = Source.StartActivity(ProcessSpanName, ActivityKind.Consumer);

            return rooted;
        }
        finally
        {
            Activity.Current = ambient;
        }
    }
}
