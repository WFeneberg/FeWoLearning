using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.WebServices;

// Exercise 049 — BackgroundServiceInstrumentation (web-services).
// Goal:   Instrument something with no request to hang a trace off, without producing one
//         span that never ends.
// Drills: a root activity per unit of work, error status per iteration, not leaking
//         ambient context between them.
// Passes: N items produce N spans, each named for the work and tagged with its item;
//         every one of them is a ROOT - no parent, and N DIFFERENT trace ids;
//         an item that throws still ends its span with Error status, and the loop carries
//                     on to the next one;
//         and Activity.Current is null between iterations.
//
// The second clause is the row. The obvious thing to do in a worker is open a span when
// the service starts, and it is wrong in a way that gets worse the longer the process
// lives: one span that is still running after a week, one trace holding every item the
// worker has ever touched, and a backend that will not show you any of it because the
// trace has no end and no reasonable size. Nothing errors. It simply never appears.
//
// A unit of work is a trace. For a worker that unit is one item, one message, one tick -
// and each gets its own root, exactly as an HTTP request would if there were one.
//
// The third clause is the difference between instrumentation and control flow, again -
// row 018 made the same point about rethrowing. A worker that dies on the first bad item
// is a worker that stops; a worker that swallows the failure silently is worse. Record it
// on that item's span, and go on to the next.
//
// The fourth is the leak that makes the other three lie. Activity.Current is ambient and
// AsyncLocal: an iteration that leaves its span open makes the NEXT iteration a child of
// it, and the roots quietly turn into the single growing trace this row exists to
// prevent.
public static class Ex049_BackgroundServiceInstrumentation
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex049";

    /// <summary>The name of the span each iteration opens.</summary>
    public const string IterationSpanName = "process";

    /// <summary>The attribute naming which item an iteration handled.</summary>
    public const string ItemAttribute = "work.item";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// Build a provider recording <see cref="SourceName"/> into
    /// <paramref name="exported"/>. The caller disposes it.
    /// </summary>
    public static TracerProvider Build(ICollection<Activity> exported) =>
        Sdk.CreateTracerProviderBuilder()
            .AddSource(SourceName)
            .AddInMemoryExporter(exported)
            .Build();

    /// <summary>
    /// Run <paramref name="work"/> once per entry in <paramref name="items"/>.
    ///
    /// Each iteration gets its OWN root span named <see cref="IterationSpanName"/>,
    /// tagged <see cref="ItemAttribute"/> with that item. An iteration whose work throws
    /// records the failure on its span - status <see cref="ActivityStatusCode.Error"/>
    /// with the exception's message - and the loop continues.
    ///
    /// This method never throws, and leaves <see cref="Activity.Current"/> as it found it.
    /// </summary>
    public static async Task RunAsync(IEnumerable<string> items, Func<string, Task> work)
    {
        foreach (var item in items)
        {
            // Clearing Activity.Current is the ONLY thing that forces a root. Measured
            // 2026-09-06: passing `parentContext: default` does NOT - the activity still
            // inherits whatever is ambient - and neither does `parentId: null`. Both look
            // like they should, and both quietly nest.
            var ambient = Activity.Current;
            Activity.Current = null;

            Activity? activity;
            try
            {
                activity = Source.StartActivity(IterationSpanName);
                activity?.SetTag(ItemAttribute, item);

                try
                {
                    await work(item);
                }
                catch (Exception failure)
                {
                    // Recorded on this item's span, and then the loop goes on. A worker
                    // that dies on the first bad item is a worker that stops.
                    activity?.AddException(failure);
                    activity?.SetStatus(ActivityStatusCode.Error, failure.Message);
                }

                activity?.Dispose();
            }
            finally
            {
                // Whatever the caller had is handed back exactly as it was found.
                Activity.Current = ambient;
            }
        }
    }
}
