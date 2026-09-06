using System.Diagnostics;
using FeWoLearning.Telemetry.Exercises.Diagnostics;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex017_TagsBaggageEventsTests
{
    private static (Activity Request, Activity Handler) Run(TraceProbe probe)
    {
        Ex017_TagsBaggageEvents.HandleRequest("acme", "eu-central", retries: 2);

        return (
            probe.Stopped.Single(a => a.DisplayName == Ex017_TagsBaggageEvents.RequestName),
            probe.Stopped.Single(a => a.DisplayName == Ex017_TagsBaggageEvents.HandlerName));
    }

    [Fact]
    public void Adversarial_A_A_tag_belongs_to_one_activity_and_does_not_inherit()
    {
        // The direction people get wrong most often. Setting deployment.region on a
        // parent does nothing for its children, so the child span arrives at the
        // backend without it and the dashboard filtered on region silently loses half
        // its rows.
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex017_TagsBaggageEvents.SourceName);

        var (request, handler) = Run(probe);

        Assert.Equal("eu-central", request.GetTagItem(Ex017_TagsBaggageEvents.RegionTagKey)?.ToString());
        Assert.Null(handler.GetTagItem(Ex017_TagsBaggageEvents.RegionTagKey));
    }

    [Fact]
    public void Baggage_set_on_the_parent_is_readable_from_the_child()
    {
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex017_TagsBaggageEvents.SourceName);

        var (_, handler) = Run(probe);

        Assert.Equal("acme", handler.GetBaggageItem(Ex017_TagsBaggageEvents.TenantBaggageKey));
    }

    [Fact]
    public void Adversarial_B_Baggage_is_context_not_a_span_attribute()
    {
        // The shortcut this catches is doing both - AddBaggage AND SetTag - which
        // makes every behavioural fact pass while hiding whether the learner knew
        // which mechanism did the work.
        //
        // It is also true in production: baggage is not indexed by any backend unless
        // something deliberately copies it onto a span. And it travels, to every
        // outbound request for the rest of the trace - which is exactly why a tenant
        // id is fine in there and anything personal is a leak with a long reach.
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex017_TagsBaggageEvents.SourceName);

        var (request, handler) = Run(probe);

        Assert.Null(request.GetTagItem(Ex017_TagsBaggageEvents.TenantBaggageKey));
        Assert.Null(handler.GetTagItem(Ex017_TagsBaggageEvents.TenantBaggageKey));
    }

    [Fact]
    public void One_event_is_recorded_per_retry_carrying_its_attempt_number()
    {
        // An event is the right shape for "this happened, n times, at these moments".
        // A tag could only say "n" and would lose the when.
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex017_TagsBaggageEvents.SourceName);

        var (_, handler) = Run(probe);

        var events = handler.Events.ToArray();
        Assert.Equal(2, events.Length);
        Assert.All(events, e => Assert.Equal(Ex017_TagsBaggageEvents.RetryEventName, e.Name));
        Assert.Equal(
            ["1", "2"],
            events.Select(e =>
                e.Tags.Single(t => t.Key == Ex017_TagsBaggageEvents.AttemptTagKey).Value?.ToString()));
    }

    [Fact]
    public void The_handler_is_a_child_of_the_request()
    {
        // Baggage inheritance is a property of the parent/child relationship. Without
        // it the "baggage reaches the child" fact would be measuring nothing.
        using var ctx = new TelemetryContext();
        using var probe = new TraceProbe(Ex017_TagsBaggageEvents.SourceName);

        var (request, handler) = Run(probe);

        Assert.Equal(request.SpanId, handler.ParentSpanId);
    }
}
