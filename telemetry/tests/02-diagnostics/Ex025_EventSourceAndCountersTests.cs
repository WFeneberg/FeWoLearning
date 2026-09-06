using System.Diagnostics.Tracing;
using FeWoLearning.Telemetry.Exercises.Diagnostics;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex025_EventSourceAndCountersTests
{
    /// <summary>
    /// Subscribes to the exercise's source at a chosen level and keyword mask, and
    /// keeps what arrives.
    /// </summary>
    private sealed class Subscriber : EventListener
    {
        private readonly List<EventWrittenEventArgs> _events = [];
        private EventSource? _target;
        private EventLevel _level;
        private EventKeywords _keywords;
        private bool _configured;

        public Subscriber(EventLevel level, EventKeywords keywords)
        {
            // OnEventSourceCreated runs during the BASE constructor, before a single
            // line of this body - so every already-existing source is announced while
            // _level and _keywords are still default. Enabling there would subscribe
            // with the wrong mask and nothing would say so.
            //
            // Hence the two-step: remember the source during construction, subscribe
            // once the settings actually exist.
            _level = level;
            _keywords = keywords;
            _configured = true;

            if (_target is not null) EnableEvents(_target, level, keywords);
        }

        public IReadOnlyList<EventWrittenEventArgs> Events
        {
            get { lock (_events) return _events.ToArray(); }
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name != Ex025_EventSourceAndCounters.SourceName) return;

            _target = eventSource;

            // Sources created LATER arrive here too, and by then the settings are real.
            if (_configured) EnableEvents(eventSource, _level, _keywords);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventSource.Name != Ex025_EventSourceAndCounters.SourceName) return;

            lock (_events) _events.Add(eventData);
        }
    }

    private const EventKeywords All = (EventKeywords)(-1);

    [Fact]
    public void The_ingest_event_arrives_with_its_id_level_and_payload()
    {
        using var subscriber = new Subscriber(EventLevel.Verbose, All);

        Ex025_EventSourceAndCounters.Log.BatchIngested("b-1", 42);

        var written = Assert.Single(subscriber.Events);
        Assert.Equal(1, written.EventId);
        Assert.Equal(EventLevel.Informational, written.Level);
        Assert.Equal(new object[] { "b-1", 42 }, written.Payload!);
    }

    [Fact]
    public void The_storage_event_arrives_with_its_own_id_and_level()
    {
        using var subscriber = new Subscriber(EventLevel.Verbose, All);

        Ex025_EventSourceAndCounters.Log.StoragePressure(91);

        var written = Assert.Single(subscriber.Events);
        Assert.Equal(2, written.EventId);
        Assert.Equal(EventLevel.Warning, written.Level);
        Assert.Equal(new object[] { 91 }, written.Payload!);
    }

    [Fact]
    public void Adversarial_A_A_keyword_mask_selects_one_subsystem_and_leaves_the_rest()
    {
        // Why keywords are worth the trouble: the subscriber says what it wants and the
        // runtime filters AT THE SOURCE, so the events it did not ask for are never
        // constructed, never serialised, never transferred. That is what makes it
        // affordable to leave this instrumentation in permanently.
        //
        // It is also the thing that goes wrong quietly. Get the keyword wrong and a
        // consumer receives nothing at all and concludes the code never ran.
        using var subscriber = new Subscriber(
            EventLevel.Verbose, Ex025_EventSourceAndCounters.Keywords.Ingest);

        Ex025_EventSourceAndCounters.Log.BatchIngested("b-1", 42);
        Ex025_EventSourceAndCounters.Log.StoragePressure(91);

        Assert.Equal([1], subscriber.Events.Select(e => e.EventId));
    }

    [Fact]
    public void Adversarial_B_The_level_filter_keeps_the_warning_and_drops_the_information()
    {
        // The other axis, and the one whose direction is easy to invert: EventLevel
        // counts UP as it gets less severe (Critical 1 ... Informational 4, Verbose 5),
        // so enabling at Warning admits everything at or below 3 and excludes
        // Informational.
        using var subscriber = new Subscriber(EventLevel.Warning, All);

        Ex025_EventSourceAndCounters.Log.BatchIngested("b-1", 42);
        Ex025_EventSourceAndCounters.Log.StoragePressure(91);

        Assert.Equal([2], subscriber.Events.Select(e => e.EventId));
    }

    [Fact]
    public void Discovery_finds_the_runtimes_own_event_source()
    {
        // The reason this row sits beside Meter rather than being replaced by it:
        // EventSource is not an alternative you choose, it is the channel that is
        // already there. The runtime, the thread pool, the GC, Kestrel, HttpClient and
        // EF Core all publish on it - so being able to subscribe is how you answer
        // questions about code you did not write.
        var sources = Ex025_EventSourceAndCounters.DiscoverEventSources();

        Assert.Contains(Ex025_EventSourceAndCounters.RuntimeSourceName, sources);
        Assert.Contains(Ex025_EventSourceAndCounters.SourceName, sources);
    }
}
