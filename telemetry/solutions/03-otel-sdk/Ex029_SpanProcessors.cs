using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace FeWoLearning.Telemetry.Exercises.Otel;

// Exercise 029 — SpanProcessors (otel-sdk).
// Goal:   Get between the span and the exporter, which is where enrichment, filtering
//         and redaction actually live.
// Drills: BaseProcessor<Activity>, OnStart and OnEnd, processor ordering.
// Passes: one processor sees OnStart before the work and OnEnd after it;
//         OnStart runs on a span that has not finished, OnEnd on one that has;
//         both of its tags reach the exported span;
//         and with two processors, BOTH OnStart and OnEnd run in REGISTRATION order -
//                     the chain does not unwind.
//
// The last clause is the one that surprises everyone who has written middleware. An
// ASP.NET pipeline nests: outer-in, inner-in, inner-out, outer-out. A processor chain
// does not - OTel composes processors into a list and walks it head to tail for both
// hooks. So "the last processor gets the final say" is only true for OnEnd because it
// is last in the list, not because anything unwound, and a processor added after the
// exporter runs after the export rather than around it.
//
// OnStart is where you attach what is knowable at the start - the tenant from ambient
// context, a deployment attribute - and it is the only hook whose work is visible to
// everything downstream in the span's own lifetime. OnEnd is where you attach what is
// only knowable at the end, and where filtering belongs.
//
// A warning that this row's own tests had to be written around, and which applies to
// every test in this track: the in-memory exporter stores the Activity OBJECT, not a
// snapshot of it. A processor that mutates a span after it was exported still changes
// what the test sees. Ordering is therefore graded on the call log, which is
// unforgeable, and never on which tags happen to be present at the end.
public static class Ex029_SpanProcessors
{
    /// <summary>The source this exercise emits from.</summary>
    public const string SourceName = "fewolearning.telemetry.ex029";

    /// <summary>The name of every span this exercise starts.</summary>
    public const string WorkSpanName = "work";

    /// <summary>Stamped by a processor in OnStart, suffixed with its label.</summary>
    public const string StartedTagPrefix = "processor.started.";

    /// <summary>Stamped by a processor in OnEnd, suffixed with its label.</summary>
    public const string EndedTagPrefix = "processor.ended.";

    /// <summary>The one source this exercise emits from.</summary>
    public static ActivitySource Source { get; } = new(SourceName);

    /// <summary>
    /// A processor that, for every span:
    ///
    ///   - on start, appends "<c>{label}:start</c>" to <paramref name="log"/> and sets
    ///     the tag <see cref="StartedTagPrefix"/> + label to "yes";
    ///   - on end, appends "<c>{label}:end</c>" and sets
    ///     <see cref="EndedTagPrefix"/> + label to "yes".
    /// </summary>
    public static BaseProcessor<Activity> CreateEnrichingProcessor(string label, IList<string> log) =>
        new EnrichingProcessor(label, log);

    private sealed class EnrichingProcessor(string label, IList<string> log) : BaseProcessor<Activity>
    {
        public override void OnStart(Activity data)
        {
            log.Add($"{label}:start");
            data.SetTag(StartedTagPrefix + label, "yes");
        }

        public override void OnEnd(Activity data)
        {
            log.Add($"{label}:end");
            data.SetTag(EndedTagPrefix + label, "yes");
        }
    }

    /// <summary>
    /// Build a provider listening to <see cref="SourceName"/>, with
    /// <paramref name="processors"/> added in the order given and the in-memory
    /// exporter added LAST, so nothing runs after the export.
    ///
    /// The caller disposes it.
    /// </summary>
    public static TracerProvider Build(
        ICollection<Activity> exported, params BaseProcessor<Activity>[] processors)
    {
        var builder = Sdk.CreateTracerProviderBuilder().AddSource(SourceName);

        // Registration order IS execution order, for both hooks. Anything added after
        // the exporter runs after the export rather than around it.
        foreach (var processor in processors) builder.AddProcessor(processor);

        return builder.AddInMemoryExporter(exported).Build();
    }
}
