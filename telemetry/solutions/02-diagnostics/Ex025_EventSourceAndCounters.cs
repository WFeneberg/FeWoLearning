using System.Diagnostics.Tracing;

namespace FeWoLearning.Telemetry.Exercises.Diagnostics;

// Exercise 025 — EventSourceAndCounters (diagnostics).
// Goal:   Emit on the runtime's own diagnostics channel - the one the runtime, ASP.NET
//         Core and EF Core already use - and subscribe to it.
// Drills: EventSource, [Event] ids and levels, keywords, EventListener.
// Passes: an enabled listener receives BatchIngested as event 1 at Informational, with
//                     the batch id and row count as its payload, in that order;
//         it receives StoragePressure as event 2 at Warning;
//         enabling only the Ingest keyword delivers the first and NOT the second;
//         enabling at Warning delivers the second and NOT the first;
//         and DiscoverEventSources finds the runtime's own "System.Runtime" source.
//
// Why this exists next to Meter, which is the modern way to publish numbers: EventSource
// is not an alternative you choose, it is the channel that is ALREADY THERE. The
// runtime, the thread pool, the GC, Kestrel, HttpClient and EF Core all publish on it,
// and dotnet-counters, dotnet-trace and every profiler read it. Being able to subscribe
// is how you answer questions about code you did not write - which is most of the
// interesting ones.
//
// The third and fourth clauses are the reason keywords and levels are worth the trouble.
// A subscriber says what it wants and the runtime does the filtering AT THE SOURCE, so
// the events it did not ask for are never constructed, never serialised, never
// transferred. That is what makes it affordable to leave this instrumentation in
// permanently. Get the keywords wrong and a consumer either drowns or, far more likely,
// silently receives nothing and concludes the code never ran.
//
// The id in WriteEvent must match the id in [Event]. Nothing checks this for you at run
// time; the payload simply arrives labelled as a different event.
[EventSource(Name = SourceName)]
public sealed class Ex025_EventSourceAndCounters : EventSource
{
    /// <summary>The name a subscriber uses to find this source.</summary>
    public const string SourceName = "FeWoLearning-Telemetry-Ex025";

    /// <summary>The runtime's own source, used by DiscoverEventSources' fact.</summary>
    public const string RuntimeSourceName = "System.Runtime";

    /// <summary>The one shared instance. An EventSource is a singleton by convention.</summary>
    public static readonly Ex025_EventSourceAndCounters Log = new();

    /// <summary>Keywords let a subscriber take one subsystem and leave the rest.</summary>
    public static class Keywords
    {
        /// <summary>Everything about taking data in.</summary>
        public const EventKeywords Ingest = (EventKeywords)0x1;

        /// <summary>Everything about where it is kept.</summary>
        public const EventKeywords Storage = (EventKeywords)0x2;
    }

    private Ex025_EventSourceAndCounters()
    {
    }

    /// <summary>
    /// Event 1, Informational, keyword <see cref="Keywords.Ingest"/>. Payload is
    /// <paramref name="batchId"/> then <paramref name="rows"/>.
    /// </summary>
    [Event(1, Level = EventLevel.Informational, Keywords = Keywords.Ingest,
        Message = "Batch {0} ingested with {1} rows")]
    public void BatchIngested(string batchId, int rows) =>
        // The id here MUST match the id in [Event]. Nothing checks it at run time; the
        // payload simply arrives labelled as a different event.
        WriteEvent(1, batchId, rows);

    /// <summary>
    /// Event 2, Warning, keyword <see cref="Keywords.Storage"/>. Payload is
    /// <paramref name="percent"/>.
    /// </summary>
    [Event(2, Level = EventLevel.Warning, Keywords = Keywords.Storage,
        Message = "Storage at {0} percent")]
    public void StoragePressure(int percent) => WriteEvent(2, percent);

    /// <summary>
    /// Every EventSource currently alive in this process, by name.
    ///
    /// A listener is told about all existing sources the moment it is constructed, and
    /// about later ones as they appear - which is how a tool discovers what it can
    /// subscribe to without knowing any of it in advance.
    /// </summary>
    [NonEvent]
    public static IReadOnlyList<string> DiscoverEventSources()
    {
        // Touch our own singleton first: a source that has never been CONSTRUCTED has
        // never been announced, so it would be missing from a list that is otherwise
        // complete.
        _ = Log;

        using var discovery = new DiscoveryListener();
        return discovery.Names;
    }

    /// <summary>
    /// Collects the name of every source announced to it. A listener is told about all
    /// existing sources the moment it is constructed - which is how a tool discovers
    /// what it can subscribe to without knowing any of it in advance.
    /// </summary>
    private sealed class DiscoveryListener : EventListener
    {
        // Deliberately WITHOUT an initializer. OnEventSourceCreated runs during the
        // base constructor, before any field initializer in this class - so
        // `= []` here would run afterwards and silently wipe every name collected
        // during construction, which is most of them.
        private List<string>? names;

        public IReadOnlyList<string> Names
        {
            get { lock (this) return names?.ToArray() ?? []; }
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            lock (this)
            {
                names ??= [];
                names.Add(eventSource.Name);
            }
        }
    }
}
