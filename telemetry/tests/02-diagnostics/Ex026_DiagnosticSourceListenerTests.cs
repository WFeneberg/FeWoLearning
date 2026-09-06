using FeWoLearning.Telemetry.Exercises.Diagnostics;

namespace FeWoLearning.Telemetry.Tests.Diagnostics;

public class Ex026_DiagnosticSourceListenerTests
{
    private sealed class Counted
    {
        public int Calls { get; private set; }

        public int Count()
        {
            Calls++;
            return 3;
        }
    }

    private sealed class Captured
    {
        private readonly List<(string Name, object? Payload)> _events = [];

        public void Add(string name, object? payload)
        {
            lock (_events) _events.Add((name, payload));
        }

        public IReadOnlyList<(string Name, object? Payload)> Events
        {
            get { lock (_events) return _events.ToArray(); }
        }
    }

    /// <summary>
    /// Reads a named property off an anonymous payload, the way every DiagnosticSource
    /// consumer has to. There is no schema and no interface - only a property name the
    /// consumer was told about in documentation.
    /// </summary>
    private static object? Property(object? payload, string name) =>
        payload?.GetType().GetProperty(name)?.GetValue(payload);

    [Fact]
    public void Adversarial_A_With_nobody_subscribed_nothing_is_written_or_even_built()
    {
        // Why IsEnabled is not optional here. An ActivitySource costs nothing when
        // unheard because the runtime hands you a null; a DiagnosticSource hands you
        // nothing at all, so the payload is an object YOU allocate at the call site.
        // Write unconditionally and every request builds an anonymous object nobody
        // reads.
        var work = new Counted();

        Ex026_DiagnosticSourceListener.ProcessOrder("O-42", work.Count);

        Assert.Equal(0, work.Calls);
    }

    [Fact]
    public void A_subscriber_receives_the_start_and_stop_events_in_order()
    {
        var work = new Counted();
        var captured = new Captured();

        using (Ex026_DiagnosticSourceListener.Subscribe(_ => true, captured.Add))
        {
            Ex026_DiagnosticSourceListener.ProcessOrder("O-42", work.Count);
        }

        Assert.Equal(
            [Ex026_DiagnosticSourceListener.StartEventName, Ex026_DiagnosticSourceListener.StopEventName],
            captured.Events.Select(e => e.Name));
        Assert.Equal(1, work.Calls);
    }

    [Fact]
    public void The_payloads_expose_the_documented_properties()
    {
        // The trade this channel makes. There is no schema: the payload is an object
        // and the consumer reads it by reflection. That is what lets it carry an
        // HttpRequestMessage or a DbCommand whole - and why renaming a property is a
        // silent break for every consumer, since nothing fails to compile and the value
        // simply arrives as null forever.
        var captured = new Captured();

        using (Ex026_DiagnosticSourceListener.Subscribe(_ => true, captured.Add))
        {
            Ex026_DiagnosticSourceListener.ProcessOrder("O-42", () => 3);
        }

        var start = captured.Events[0].Payload;
        var stop = captured.Events[1].Payload;

        Assert.Equal("O-42", Property(start, "OrderId"));
        Assert.Equal("O-42", Property(stop, "OrderId"));
        Assert.Equal(3, Property(stop, "ItemCount"));
    }

    [Fact]
    public void Adversarial_B_The_predicate_gates_each_event_by_name()
    {
        // IsEnabled is checked per event NAME, which is what makes it possible for a
        // consumer to take the stop event and skip the start. An implementation that
        // checks once, or checks the source rather than the event, delivers both.
        var work = new Counted();
        var captured = new Captured();

        using (Ex026_DiagnosticSourceListener.Subscribe(
            name => name == Ex026_DiagnosticSourceListener.StopEventName, captured.Add))
        {
            Ex026_DiagnosticSourceListener.ProcessOrder("O-42", work.Count);
        }

        var only = Assert.Single(captured.Events);
        Assert.Equal(Ex026_DiagnosticSourceListener.StopEventName, only.Name);

        // The stop event needs the count, so the work still happened - once.
        Assert.Equal(1, work.Calls);
    }

    [Fact]
    public void Adversarial_C_The_subscription_ends_when_it_is_disposed()
    {
        // A DiagnosticListener subscription is a live IObservable subscription, and a
        // leaked one keeps receiving for the life of the process - which is how a test
        // suite ends up with one consumer per test and counts that grow every run.
        var captured = new Captured();

        var subscription = Ex026_DiagnosticSourceListener.Subscribe(_ => true, captured.Add);
        Ex026_DiagnosticSourceListener.ProcessOrder("O-42", () => 3);
        subscription.Dispose();
        Ex026_DiagnosticSourceListener.ProcessOrder("O-43", () => 3);

        Assert.Equal(2, captured.Events.Count);
        Assert.All(captured.Events, e => Assert.Equal("O-42", Property(e.Payload, "OrderId")));
    }
}
